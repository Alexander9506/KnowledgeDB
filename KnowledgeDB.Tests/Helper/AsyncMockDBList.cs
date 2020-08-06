using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KnowledgeDB.Tests.Helper
{

    internal static class MockAsyncQuery
    {
        // Return a DbSet of the specified generic type with support for async operations
        public static Mock<DbSet<T>> GetDbSet<T>(this IQueryable<T> testData) where T : class
        {
            var MockSet = new Mock<DbSet<T>>();
            MockSet.As<IAsyncEnumerable<T>>().Setup(x => x.GetAsyncEnumerator(default)).Returns(new MockAsyncEnumerator<T>(testData.GetEnumerator()));
            MockSet.As<IQueryable<T>>().Setup(x => x.Provider).Returns(new MockAsyncQueryProvider<T>(testData.Provider));
            MockSet.As<IQueryable<T>>().Setup(x => x.Expression).Returns(testData.Expression);
            MockSet.As<IQueryable<T>>().Setup(x => x.ElementType).Returns(testData.ElementType);
            MockSet.As<IQueryable<T>>().Setup(x => x.GetEnumerator()).Returns(testData.GetEnumerator());
            MockSet.As<IEnumerable<T>>().Setup(x => x.GetEnumerator()).Returns(testData.GetEnumerator());
            return MockSet;
        }
    }

    // Async query provider for unit testing
    internal class MockAsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            internal MockAsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new MockAsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new MockAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return _inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }

            public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
            {
                return new MockAsyncEnumerable<TResult>(expression);
            }

            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                var expectedResultType = typeof(TResult).GetGenericArguments()[0];
                var executionResult = typeof(IQueryProvider)
                    .GetMethod(
                        name: nameof(IQueryProvider.Execute),
                        genericParameterCount: 1,
                        types: new[] { typeof(Expression) })
                    .MakeGenericMethod(expectedResultType)
                    .Invoke(this, new[] { expression });

                return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                    .MakeGenericMethod(expectedResultType)
                    .Invoke(null, new[] { executionResult });
            }
        }

        // Async enumerable for unit testing
        internal class MockAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public MockAsyncEnumerable(IEnumerable<T> enumerable)
                : base(enumerable)
            { }

            public MockAsyncEnumerable(Expression expression)
                : base(expression)
            { }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken)
            {
                return GetEnumerator();
            }

            public IAsyncEnumerator<T> GetEnumerator()
            {
                return new MockAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }

            IQueryProvider IQueryable.Provider {
                get { return new MockAsyncQueryProvider<T>(this); }
            }
        }

        // Async enumerator for unit testing
        internal class MockAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public MockAsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner;
            }

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return new ValueTask();
            }

            public T Current {
                get {
                    return _inner.Current;
                }
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(_inner.MoveNext());
            }
        }

}

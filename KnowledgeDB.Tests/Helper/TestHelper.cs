using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace KnowledgeDB.Tests.Helper
{
    public static class TestHelper
    {
        public static T GetViewModel<T>(this IActionResult result) where T : class
        {
            return (result as ViewResult).Model as T;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KnowledgeDB.Areas.Identity.Data;
using KnowledgeDB.Infrastructure;
using KnowledgeDB.Models;
using KnowledgeDB.Models.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace KnowledgeDB.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<KnowledgeDBUser> _userManager;
        private readonly SignInManager<KnowledgeDBUser> _signInManager;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IFileRepository _fileRepository;
        private readonly IFileHelper _fileHelper;

        public IndexModel(
            UserManager<KnowledgeDBUser> userManager,
            SignInManager<KnowledgeDBUser> signInManager,
            IWebHostEnvironment environment,
            IConfiguration configuration,
            IFileRepository fileRepository,
            IFileHelper fileHelper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
            _configuration = configuration;
            _fileRepository = fileRepository;
            _fileHelper = fileHelper;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            [Display(Name ="Profile Picture")]
            public string ProfilePicturePath { get; set; }
        }

        private async Task LoadAsync(KnowledgeDBUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                ProfilePicturePath = user.ProfilePicturePath,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();

                if (await DeleteProfilePictureAsync(user))
                {
                    FileContainer fileContainer = await SaveProfilePictureAsync(file);
                    if (fileContainer != null)
                    {
                        user.ProfilePicturePath = fileContainer.FilePathFull;
                        await _userManager.UpdateAsync(user);
                    }
                    else
                    {
                        StatusMessage = "Unexpected error when trying to set Profile Picture.";
                        return RedirectToPage();
                    }
                }
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        private async Task<FileContainer> SaveProfilePictureAsync(IFormFile file)
        {
            String basePath = Path.Combine(_environment.WebRootPath, _configuration.GetValue<String>("ProfileImagePath"));

            IEnumerable<IFormFile> files = new List<IFormFile> { file } ;
            IEnumerable<FileContainer> savedFiles = await _fileHelper.SaveFromFormFiles(files, basePath);

            return savedFiles.FirstOrDefault();
        }

        private async Task<bool> DeleteProfilePictureAsync(KnowledgeDBUser user)
        {
            if(user != null && !String.IsNullOrWhiteSpace(user.ProfilePicturePath))
            {
                FileContainer profilePicture = _fileRepository.FileContainers.Where(fc => fc.FilePathFull == user.ProfilePicturePath).FirstOrDefault()   ;

                return await _fileHelper.DeleteFileAsync(profilePicture);
            }
            return true;
        }
    }
}

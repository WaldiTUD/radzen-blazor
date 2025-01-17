﻿using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Radzen.Blazor
{
    /// <summary>
    /// Class RadzenLogin.
    /// Implements the <see cref="Radzen.RadzenComponent" />
    /// </summary>
    /// <seealso cref="Radzen.RadzenComponent" />
    public partial class RadzenLogin : RadzenComponent
    {
        /// <summary>
        /// Gets or sets a value indicating whether [automatic complete].
        /// </summary>
        /// <value><c>true</c> if [automatic complete]; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool AutoComplete { get; set; } = true;

        /// <summary>
        /// Gets the component CSS class.
        /// </summary>
        /// <returns>System.String.</returns>
        protected override string GetComponentCssClass()
        {
            return "login";
        }

        /// <summary>
        /// The username
        /// </summary>
        string _username;
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        [Parameter]
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                if (_username != value)
                {
                    _username = value;
                }
            }
        }

        /// <summary>
        /// The password
        /// </summary>
        string _password;
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        [Parameter]
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (_password != value)
                {
                    _password = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the login.
        /// </summary>
        /// <value>The login.</value>
        [Parameter]
        public EventCallback<Radzen.LoginArgs> Login { get; set; }

        /// <summary>
        /// Gets or sets the register.
        /// </summary>
        /// <value>The register.</value>
        [Parameter]
        public EventCallback Register { get; set; }

        /// <summary>
        /// Gets or sets the reset password.
        /// </summary>
        /// <value>The reset password.</value>
        [Parameter]
        public EventCallback<string> ResetPassword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow register].
        /// </summary>
        /// <value><c>true</c> if [allow register]; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool AllowRegister { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether [allow reset password].
        /// </summary>
        /// <value><c>true</c> if [allow reset password]; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool AllowResetPassword { get; set; } = true;

        /// <summary>
        /// Gets or sets the login text.
        /// </summary>
        /// <value>The login text.</value>
        [Parameter]
        public string LoginText { get; set; } = "Login";

        /// <summary>
        /// Gets or sets the register text.
        /// </summary>
        /// <value>The register text.</value>
        [Parameter]
        public string RegisterText { get; set; } = "Sign up";

        /// <summary>
        /// Gets or sets the register message text.
        /// </summary>
        /// <value>The register message text.</value>
        [Parameter]
        public string RegisterMessageText { get; set; } = "Don't have an account yet?";

        /// <summary>
        /// Gets or sets the reset password text.
        /// </summary>
        /// <value>The reset password text.</value>
        [Parameter]
        public string ResetPasswordText { get; set; } = "Forgot password";

        /// <summary>
        /// Gets or sets the user text.
        /// </summary>
        /// <value>The user text.</value>
        [Parameter]
        public string UserText { get; set; } = "Username";

        /// <summary>
        /// Gets or sets the user required.
        /// </summary>
        /// <value>The user required.</value>
        [Parameter]
        public string UserRequired { get; set; } = "Username is required";

        /// <summary>
        /// Gets or sets the password text.
        /// </summary>
        /// <value>The password text.</value>
        [Parameter]
        public string PasswordText { get; set; } = "Password";

        /// <summary>
        /// Gets or sets the password required.
        /// </summary>
        /// <value>The password required.</value>
        [Parameter]
        public string PasswordRequired { get; set; } = "Password is required";

        /// <summary>
        /// Called when [login].
        /// </summary>
        protected async Task OnLogin()
        {
            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                await Login.InvokeAsync(new Radzen.LoginArgs { Username = Username, Password = Password });
            }
        }

        /// <summary>
        /// Handles the <see cref="E:Reset" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected async Task OnReset(EventArgs args)
        {
            if (!string.IsNullOrEmpty(Username))
            {
                await ResetPassword.InvokeAsync(Username);
            }
        }

        /// <summary>
        /// Called when [register].
        /// </summary>
        protected async Task OnRegister()
        {
            await Register.InvokeAsync(EventArgs.Empty);
        }
    }
}
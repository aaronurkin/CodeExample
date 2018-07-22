using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Infrastructure
{
    /// <summary>
    /// Data transfer object for <see cref="IdentityResult"/> extending it with Data property to pass data up to Presentation Layer
    /// </summary>
    public class ServiceResult
    {
        /// <summary>
        /// Copies properties from passed <see cref="IdentityResult"/> and adds data to be passed up to Presentation Layer if needed
        /// </summary>
        /// <param name="result"><see cref="IdentityResult"/></param>
        /// <param name="data">Data to pass up to Presentation Layer</param>
        public ServiceResult(IdentityResult result, object data = null)
        {
            this.Data = data;
            this.Errors = result.Errors;
            this.Succeeded = result.Succeeded;
        }

        /// <summary>
        /// Copies properties from passed <see cref="SignInResult"/> and adds data to be passed up to Presentation Layer if needed
        /// </summary>
        /// <param name="result"><see cref="SignInResult"/></param>
        /// <param name="data">Data to pass up to Presentation Layer</param>
        public ServiceResult(SignInResult result, object data = null)
        {
            this.Data = data;
            this.Succeeded = result.Succeeded;
        }

        public object Data { get; }

        public bool Succeeded { get; }

        public IEnumerable<IdentityError> Errors { get; }

        /// <summary>
        /// Generates unsuccessful <see cref="ServiceResult"/>
        /// </summary>
        /// <param name="errors"><see cref="IdentityError[]"/> to pass up to Presentation Layer</param>
        /// <returns>New unsuccessful <see cref="ServiceResult"/> instance</returns>
        public static ServiceResult Failed(params IdentityError[] errors)
        {
            return new ServiceResult(IdentityResult.Failed(errors));
        }

        /// <summary>
        /// Generates successful <see cref="ServiceResult"/>
        /// </summary>
        /// <param name="data">Data to pass up to Presentation Layer</param>
        /// <returns>New successful <see cref="ServiceResult"/> instance</returns>
        internal static ServiceResult Succeed(object data = null)
        {
            return new ServiceResult(IdentityResult.Success, data);
        }
    }
}

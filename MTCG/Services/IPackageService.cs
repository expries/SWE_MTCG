using System;
using System.Collections.Generic;
using MTCG.Domain;
using MTCG.Requests;
using MTCG.Results;
using MTCG.Results.Errors;

namespace MTCG.Services
{
    public interface IPackageService
    {
        public Result<Package> GetPackage(Guid packageId);

        public List<Package> GetAllPackages();

        public Result<Package> CreatePackage(Package package, string token);
    }
}
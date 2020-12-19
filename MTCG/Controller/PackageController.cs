using System;
using System.Collections.Generic;
using MTCG.Request;
using MTCG.Server;
using MTCG.Service;

namespace MTCG.Controller
{
    public class PackageController : ApiController
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        public ResponseContext Create(IEnumerable<CardCreationRequest> requests)
        {
            var result = _packageService.CreatePackage(requests);
            return result.Match(package => Ok(package.Id), Conflict, BadRequest);
        }

        public ResponseContext Get(Guid packageId)
        {
            var package = _packageService.GetPackage(packageId);
            return package.Match(Ok, NotFound);
        }

        public ResponseContext GetAll()
        {
            var packages = _packageService.GetAllPackages();
            return Ok(packages);
        }
    }
}
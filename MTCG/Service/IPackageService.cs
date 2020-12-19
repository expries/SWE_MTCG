using System;
using System.Collections.Generic;
using MTCG.ActionResult;
using MTCG.ActionResult.Errors;
using MTCG.Request;
using MTCG.Resource;

namespace MTCG.Service
{
    public interface IPackageService
    {
        public OneOf<Package, NotFound> GetPackage(Guid packageId);

        public List<Package> GetAllPackages();

        public OneOf<Package, DuplicateId, Error> CreatePackage(Package package);
        
        public OneOf<Package, DuplicateId, Error> CreatePackage(IEnumerable<CardCreationRequest> requests);
    }
}
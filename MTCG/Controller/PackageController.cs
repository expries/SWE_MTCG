using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Domain;
using MTCG.Mappers;
using MTCG.Requests;
using MTCG.Results.Errors;
using MTCG.Server;
using MTCG.Services;

namespace MTCG.Controller
{
    public class PackageController : ApiController
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        public ResponseContext Create(List<CardCreationRequest> requests, string token)
        {
            var cards = CardCreationRequestMapper.Map(requests).ToList();
            var package = Package.Create();

            foreach (var card in cards)
            {
                var addCard = package.AddCard(card);

                if (!addCard.Success)
                {
                    return BadRequest(addCard.Error);
                }
            }
            
            var result = _packageService.CreatePackage(package, token);

            if (result.Success)
            {
                package = result.Value;
                return Ok(package.Id);
            }

            if (result.HasError<NotPermitted>())
            {
                return Forbidden(result.Error);
            }

            if (result.HasError<DuplicatePackageId, DuplicateCardId>())
            {
                return Conflict(result.Error);
            }

            return BadRequest(result.Error);
        }

        public ResponseContext Get(Guid packageId)
        {
            var result = _packageService.GetPackage(packageId);

            if (!result.Success)
            {
                return NotFound(result.Error);
            }
            
            var package = result.Value;
            return Ok(package);
        }

        public ResponseContext GetAll()
        {
            var packages = _packageService.GetAllPackages();
            return Ok(packages);
        }
    }
}
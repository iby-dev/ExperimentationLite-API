﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExperimentationLite.Domain.Entities;
using ExperimentationLite.Domain.Exceptions;
using ExperimentationLite.Logic.Directors;
using ExperimentationLite.Logic.Mapper;
using ExperimentationLite.Logic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ExperimentationLite.Api.Controllers
{
    [Route("[controller]")]
    public class FeaturesController : Controller
    {
        private readonly IFeaturesDirector _director;
        private readonly IDtoToEntityMapper<BaseFeatureViewModel, Feature> _mapper;
        private readonly ILogger<FeaturesController> _logger;

        public FeaturesController(IFeaturesDirector director, 
            IDtoToEntityMapper<BaseFeatureViewModel, Feature> mapper,
            ILogger<FeaturesController> logger)
        {
            _director = director;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all available features switches found in the API.
        /// </summary>
        /// <remarks>A simple sure fire way of finding out what is inside the api - use this endpoint when administering the API.
        /// As it reveals feature switch names and ids. Go ahead and hit the 'Try it Out' button' - there are no parameters required for this
        /// action method.</remarks>
        /// <response code="200">Request processed successfully.</response>
        /// <returns>a list of feature switches.</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(ListViewModel<Feature>), 200)]
        public IActionResult GetAllFeatures()
        {
            var allFeatures = _director.GetAllFeatures();
            var model = new ListViewModel<Feature>(allFeatures);
            return Ok(model);
        }

        /// <summary>
        /// Retrieves a feature switch object by its ID value.
        /// </summary>
        /// <remarks>The 'ID' parameter should be like: e.g: 59e4c8190b637e1524aea56f</remarks>
        /// <response code="200">Requested feature switch found.</response>
        /// <response code="404">Requested feature switch not found.</response>
        /// <returns>a feature switch object.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ViewModel<Feature>), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetFeatureByIdAsync(
            [Required(AllowEmptyStrings = false, ErrorMessage = "The id parameter cannot be null or contain whitespace.")]
            Guid id)
        {
            var feature = _director.GetFeatureById(id);
            if (feature == null)
            {
                return NotFound();
            }

            var model = new ViewModel<Feature>(feature);
            return Ok(model);
        }

        /// <summary>
        /// Retrieves a feature switch object by its friendly ID value.
        /// </summary>
        /// <remarks>The 'friendlyId' parameter should be a unique non-negative numeric value like: e.g: 1 </remarks>
        /// <response code="200">Requested feature switch found.</response>
        /// <response code="404">Requested feature switch not found.</response>
        /// <returns>a feature switch object.</returns>
        [HttpGet("{friendlyId:int}")]
        [ProducesResponseType(typeof(ViewModel<Feature>), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetFeatureByFriendlyId([Required] [Range(1, int.MaxValue)] int friendlyId)
        {
            var feature = _director.GetFeatureByFriendlyId(friendlyId);
            if (feature == null)
            {
                return NotFound();
            }

            var model = new ViewModel<Feature>(feature);
            return Ok(model);
        }

        /// <summary>
        /// Retrieves a feature switch object by its name value.
        /// </summary>
        /// <remarks>The 'name' parameter should be a unique value like: e.g: 'NewMongoDB_Switch'. The style/convention you apply to the names
        /// is entirely upto but consistency is key.  </remarks>
        /// <response code="200">Requested feature switch found.</response>
        /// <response code="404">Requested feature switch not found.</response>
        /// <returns>a feature switch object.</returns>
        [HttpGet("name/{name}")]
        [ProducesResponseType(typeof(ViewModel<Feature>), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetFeatureByName(
            [Required(AllowEmptyStrings = false, ErrorMessage = "The name parameter cannot be null or contain whitespace.")]
            string name)
        {
            var feature = _director.GetFeatureByName(name);
            if (feature == null)
            {
                return NotFound();
            }

            var model = new ViewModel<Feature>(feature);
            return Ok(model);
        }

        /// <summary>
        /// Adds the new feature switch to the API.
        /// </summary>
        /// <param name="item">The feature to add.</param>
        /// <remarks>
        /// The name must be a unique value that follows some naming convention for consistency purposes.
        /// The 'friendlyId' should be a unique non-negative numeric value like: e.g: 1
        /// The bucketList can be left to empty which will default the behaviour = for all users.
        /// Otherwise provide an string based ID for a guided feature switch.
        /// </remarks>
        /// <response code="201">Feature switch created.</response>
        /// <response code="400">Invalid name or friendlyId provided for feature switch.</response>
        /// <response code="500">Internal server error.</response>
        /// <returns>a confirmation of the newly added feature switch.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Feature), 201)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult AddNewFeature([FromBody] BaseFeatureViewModel item)
        {
            var mappedFeature = _mapper.Map(item);

            try
            {
                var idOfNewlyAddedFeature = _director.AddNewFeature(mappedFeature);
                return CreatedAtAction("AddNewFeature", 
                    new
                    {
                        id = idOfNewlyAddedFeature
                    }, idOfNewlyAddedFeature);
            }
            catch (NonUniqueValueDetectedException e)
            {
                const string title = "SaveError";
                var message = $"The api was unable to save a new entity with name: {item.Name} and FriendlyId: {item.FriendlyId}.";

                ModelState.AddModelError(e.GetType().Name, e.Message);
                _logger.LogError($"{title} - {message}", e);
                _logger.LogError(e, "");
                return BadRequest(ModelState);
            }
            catch (Exception e)
            {
                const string title = "SaveError";
                const string message = "Internal Server Error - See server logs for more info.";

                ModelState.AddModelError(e.GetType().Name, e.Message);
                _logger.LogError($"{title} - {message}", e);
                _logger.LogError(e, "");
                return StatusCode(500, message);
            }
        }

        /// <summary>
        /// Updates the existing feature switch in the API.
        /// </summary>
        /// <param name="model">The feature switch to update.</param>
        /// <returns>A confirmation message.</returns>
        /// <remarks> When updating an existing switch then all fields are updated into the database apart from
        /// the ID field. This field is unique and after being generated it cannot be re-assigned or updated.
        /// 
        /// The 'Id' must be an existing id otherwise a 404 status code will be returned.
        /// Change the name field to a unique value.
        /// Change the 'friendlyId' to unique non-negative numeric value.
        /// The bucketList can be left to empty which will default the behaviour = for all users.
        /// Otherwise provide an string based identifier for a guided feature switch.
        /// </remarks>
        /// <response code="200">Feature switch updated.</response>
        /// <response code="404">Feature switch not found.</response>
        /// <response code="400">Invalid name or friendlyId provided for feature switch.</response>
        /// <response code="500">Internal Server Error.</response>
        [HttpPut("")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult UpdateExistingFeature([FromBody] FeatureViewModel model)
        {
            try
            {
                var existingFeature = _director.GetFeatureById(model.Id);
                if (existingFeature == null)
                {
                    return NotFound();
                }

                existingFeature.Name = model.Name;
                existingFeature.FriendlyId = model.FriendlyId;
                existingFeature.BucketList = model.BucketList;

                _director.UpdateFeature(existingFeature);
                return new OkObjectResult("Request processed successfully.");
            }
            catch (NonUniqueValueDetectedException e)
            {
                const string title = "UpdateError";
                var message = $"The api was unable to update entity with id: {model.Id}";

                ModelState.AddModelError(e.GetType().Name, e.Message);
                _logger.LogError($"{title} - {message}", e);
                _logger.LogError(e, "");
                return BadRequest(ModelState);
            }
            catch (Exception e)
            {
                const string title = "UpdateError";
                const string message = "Internal Server Error - See server logs for more info.";

                ModelState.AddModelError(e.GetType().Name, e.Message);
                _logger.LogError($"{title} - {message}", e);
                _logger.LogError(e, "");
                return StatusCode(500, message);
            }
        }

        /// <summary>
        /// Deletes the feature switch from the API.
        /// </summary>
        /// <param name="id">The ID of the switch to delete.</param>
        /// <returns>No Content Result when successful.</returns>
        /// <remarks>When deleting feature switches from the API simply provide the unique ID of the switch to this action method
        /// and the API will do the rest.
        /// 
        /// The 'Id' must be an existing id otherwise a 404 status code will be returned.</remarks>
        /// <response code="204">Feature switch deleted succesfully.</response>
        /// <response code="404">Feature switch not found.</response>
        /// <response code="500">Internal server error, the api has been unsuccesful in deleting the feature switch.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public IActionResult DeleteFeature(
            [Required(AllowEmptyStrings = false, ErrorMessage = "The name parameter cannot be null or contain whitespace.")]
            Guid id)
        {
            try
            {
                var result = _director.FeatureExistsById(id);
                if (result == false)
                {
                    return NotFound();
                }

                _director.DeleteFeature(id);
                return new NoContentResult();
            }
            catch (Exception e)
            {
                const string title = "DeleteError";
                var message = $"The api was unable to delete entity with id: {id}";

                ModelState.AddModelError(title, message);

                _logger.LogError($"{title} - {message}", e);
                return StatusCode(500, message); 
            }
        }

        // BUCKETS CRUD METHODS
        /// <summary>
        /// Get the bucket list on a feature by the feature Id.
        /// </summary>
        /// <param name="id">The ID of the switch to retrieve.</param>
        /// <returns>a list of bucket ids.</returns>
        /// <remarks>The 'Id' must be of an existing id otherwise a 404 status code will be returned.</remarks>
        /// <response code="200">Bucket list retrieved successfully.</response>
        /// <response code="404">Feature switch not found.</response>
        [HttpGet("{id}/bucket")]
        [ProducesResponseType(typeof(List<string>), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetBucketByFeatureId(Guid id)
        {
            var feature = _director.GetFeatureById(id);
            if (feature == null)
            {
                return NotFound();
            }

            return new OkObjectResult(feature.BucketList);
        }

        /// <summary>
        /// Returns true or false if specified bucketId is contained on bucket list found on feature switch.
        /// </summary>
        /// <param name="id">The Id of feature switch to retrieve.</param>
        /// <param name="bucketId">The bucketId to query for.</param>
        /// <returns>A true or false value if bucketId exists on bucket list.</returns>
        /// <remarks>The 'Id' must be of an existing id otherwise a 404 status code will be returned.
        /// The 'bucketId' is a string based identifier; the api will do a contains check on the bucket list to see if it is known or unknown
        /// to the bucket.</remarks>
        /// <response code="200">Bucket list queried successfully.</response>
        /// <response code="404">Feature switch not found.</response>
        [HttpGet("{id}/bucket/{bucketId}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult QueryFeatureBucketByFeatureId(Guid id, string bucketId)
        {
            var feature = _director.GetFeatureById(id);
            if (feature == null)
            {
                return NotFound();
            }

            var result = feature.BucketList.Contains(bucketId);

            return new OkObjectResult(result);
        }

        /// <summary>
        /// Adds the identifier to the feature switches bucket list.
        /// </summary>
        /// <param name="id">The feature switch id to retrieve.</param>
        /// <param name="bucketId">The bucket identifier to add to the bucket.</param>
        /// <returns>a status message based on request outcome.</returns>
        /// <remarks>The 'Id' must be of an existing id otherwise a 404 status code will be returned.
        /// the 'bucketId' is a string based identifier that will get added to the bucket list on the feature switch. Semantically this means
        /// the switch is now buided by the identifiers found only on the list.</remarks>
        /// <response code="200">Bucket list updated successfully.</response>
        /// <response code="404">Feature switch not found.</response>
        [HttpPut("{id}/bucket/{bucketId}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult AddIdToFeatureBucket(Guid id, string bucketId)
        {
            var feature = _director.GetFeatureById(id);
            if (feature == null)
            {
                return NotFound();
            }

            feature.BucketList.Add(bucketId);

            _director.UpdateFeature(feature);
            return new OkObjectResult("Request processed successfully.");
        }

        /// <summary>
        /// Removes the identifier from the feature switches bucket list.
        /// </summary>
        /// <param name="id">The id of the feature switch to retrieve.</param>
        /// <param name="bucketId">The bucket id to remove from the bucket.</param>
        /// <returns>a status message based on request outcome.</returns>
        /// <response code="200">Bucket list updated successfully.</response>
        /// <response code="404">Feature switch not found.</response>
        [HttpDelete("{id}/bucket/{bucketId}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult RemoveIdFromFeatureBucket(Guid id, string bucketId)
        {
            var feature = _director.GetFeatureById(id);
            if (feature == null)
            {
                return NotFound();
            }

            feature.BucketList.Remove(bucketId);

            _director.UpdateFeature(feature);
            return new OkObjectResult("Request processed successfully.");
        }

        /// <summary>
        /// Removes all identifiers from the feature switches bucket list.
        /// </summary>
        /// <param name="id">The id of the feature switch to retrieve.</param>
        /// <returns>a status message based on request outcome.</returns>
        /// <response code="200">Bucket list cleared successfully.</response>
        /// <response code="404">Feature switch not found.</response>
        [HttpDelete("{id}/bucket/clear")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult ClearBucketOnFeature(Guid id)
        {
            var feature = _director.GetFeatureById(id);
            if (feature == null)
            {
                return NotFound();
            }

            feature.BucketList.Clear();

            _director.UpdateFeature(feature);
            return new OkObjectResult("Request processed successfully.");
        }
    }
}
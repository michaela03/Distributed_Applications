using HotelManagmentAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using HotelManagmentAPI.Dto;
using AutoMapper;
using System.Collections.Generic;
using HotelManagmentAPI.Repository;

namespace HotelManagmentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewController(IReviewRepository reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviews()
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());
            return Ok(reviews);
        }

        [HttpGet("{reviewID}")]
        [ProducesResponseType(200, Type = typeof(ReviewDto))]
        [ProducesResponseType(400)]
        public IActionResult GetReview(int reviewID)
        {
            if (!_reviewRepository.ReviewExists(reviewID))
                return NotFound();

            var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewID));
            return Ok(review);
        }

        [HttpGet("client/{clientID}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsByClient(int clientID)
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsByClients(clientID));
            return Ok(reviews);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public IActionResult CreateReview([FromBody] ReviewDto reviewDto)
        {
            try
            {
                // Check if the request body is null
                if (reviewDto == null)
                    return BadRequest();

                // Check if a review with the same client ID already exists
                if (_reviewRepository.GetReviewsByClients(reviewDto.ClientID).Any())
                {
                    ModelState.AddModelError("", "A review for this client already exists");
                    return StatusCode(422, ModelState);
                }

                // Check if the ModelState is valid
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Map the DTO to the Review entity
                var review = _mapper.Map<Review>(reviewDto);

                // Attempt to create the review
                if (!_reviewRepository.CreateReview(review))
                {
                    ModelState.AddModelError("", "Something went wrong while saving the review");
                    return StatusCode(500, ModelState);
                }

                // Map the created review back to DTO
                var createdDto = _mapper.Map<ReviewDto>(review);

                // Return a 201 Created response with the created review DTO
                return CreatedAtAction(nameof(GetReview), new { reviewID = createdDto.ReviewID }, createdDto);
            }
            catch (Exception ex)
            {
                // Log any unhandled exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request");
            }
        }


        [HttpPut("{reviewID}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReview(int reviewID, [FromBody] ReviewDto updatedReview)
        {
            if (updatedReview == null)
                return BadRequest(ModelState);

            if (reviewID != updatedReview.ReviewID)
                return BadRequest(ModelState);

            if (!_reviewRepository.ReviewExists(reviewID))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewMap = _mapper.Map<Review>(updatedReview);

            if (!_reviewRepository.UpdateReview(reviewMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating the review");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        [HttpDelete("{clientID}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReview(int reviewID)
        {
            if (!_reviewRepository.ReviewExists(reviewID))
                return NotFound();

            var reviewToDelete = _reviewRepository.GetReview(reviewID);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewRepository.DeleteReview(reviewID))
            {
                ModelState.AddModelError("", "Something went wrong while deleting the client");
                return StatusCode(500, ModelState);
            }

            return NoContent();




        }
    }
}

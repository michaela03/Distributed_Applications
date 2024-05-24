using AutoMapper;
using HotelManagmentAPI.Dto;
using HotelManagmentAPI.Interfaces;
using HotelManagmentAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace HotelManagmentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IMapper _mapper;

        public ReservationController(IReservationRepository reservationRepository, IMapper mapper)
        {
            _reservationRepository = reservationRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetReservations()
        {
            var reservations = _reservationRepository.GetReservations();
            var reservationsDto = _mapper.Map<List<ReservationDto>>(reservations);
            return Ok(reservationsDto);
        }

        [HttpGet("{reservationID}")]
        public IActionResult GetReservation(int reservationID)
        {
            var reservation = _reservationRepository.GetReservation(reservationID);
            if (reservation == null)
                return NotFound();

            var reservationDto = _mapper.Map<ReservationDto>(reservation);
            return Ok(reservationDto);
        }

        [HttpGet("client/{clientID}")]
        public IActionResult GetReservationsByClient(int clientID)
        {
            var reservations = _reservationRepository.GetReservationsByClient(clientID);
            var reservationsDto = _mapper.Map<List<ReservationDto>>(reservations);
            return Ok(reservationsDto);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public IActionResult CreateReservation([FromBody] ReservationDto reservationDto)
        {
            if (reservationDto == null)
                return BadRequest(ModelState);

            var reservation = _reservationRepository.GetReservations()
                .Where(r => r.ClientID == reservationDto.ClientID)
                .FirstOrDefault();

            if (reservation != null)
            {
                ModelState.AddModelError("", "Reservation already exists!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resMap = _mapper.Map<Reservation>(reservationDto);

            if (!_reservationRepository.CreateReservation(resMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Succsesfully created!");

           
        }


        [HttpPut("{reservationID}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReservation(int reservationID, [FromBody] ReservationDto updatedReservation)
        {
            if (updatedReservation == null)
                return BadRequest(ModelState);

            if (reservationID != updatedReservation.ReservationID)
                return BadRequest(ModelState);

            if (!_reservationRepository.ReservationExists(reservationID))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reservationMap = _mapper.Map<Reservation>(updatedReservation);

            if (!_reservationRepository.UpdateReservation(reservationMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating the reservation");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        [HttpDelete("{clientID}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteClient(int reservationID)
        {
            if (!_reservationRepository.ReservationExists(reservationID))
                return NotFound();

            var reservationToDelete = _reservationRepository.GetReservation(reservationID);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reservationRepository.DeleteReservation(reservationID))
            {
                ModelState.AddModelError("", "Something went wrong while deleting the client");
                return StatusCode(500, ModelState);
            }

            return NoContent();



        }
    }
}

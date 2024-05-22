using HotelManagmentAPI.Data;
using HotelManagmentAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelManagmentAPI.Repository
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly DataContext _context;

        public ReservationRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<Reservation> GetReservations()
        {
            return _context.Reservations.ToList();
        }

        public Reservation GetReservation(int reservationID)
        {
            return _context.Reservations.FirstOrDefault(r => r.ReservationID == reservationID);
        }

        public ICollection<Reservation> GetReservationsByClient(int clientID)
        {
            return _context.Reservations.Where(r => r.ClientID == clientID).ToList();
        }

        public ICollection<Reservation> GetReservationsByClientName(string clientName)
        {
            return _context.Reservations.Where(r => r.Client.FirstName == clientName).ToList();
        }

        public bool ReservationExists(int reservationID)
        {
            return _context.Reservations.Any(r => r.ReservationID == reservationID);
        }

        public bool CreateReservation(Reservation reservation)
        {
            try
            {
                _context.Reservations.Add(reservation);
                return Save();
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log error)
                Console.WriteLine($"Error creating reservation: {ex.Message}");
                return false;
            }
        }

        public bool UpdateReservation(Reservation reservation)
        {
            try
            {
                _context.Entry(reservation).State = EntityState.Modified;
                return Save();
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log error)
                Console.WriteLine($"Error updating reservation: {ex.Message}");
                return false;
            }
        }

        public bool DeleteReservation(int reservationID)
        {
            try
            {
                var reservation = _context.Reservations.Find(reservationID);
                if (reservation == null)
                    return false;

                _context.Reservations.Remove(reservation);
                return Save();
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log error)
                Console.WriteLine($"Error deleting reservation: {ex.Message}");
                return false;
            }
        }

        public bool Save()
        {
            try
            {
                return _context.SaveChanges() >= 0;
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log error)
                Console.WriteLine($"Error saving changes: {ex.Message}");
                return false;
            }
        }
    }
}

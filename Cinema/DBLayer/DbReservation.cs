﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Cinema.ModelLayer;
using Cinema.DBLayer;
using System.Security.Cryptography;

namespace Cinema.DBLayer
{
    class DbReservation: IReservation
    {
        private static DbCustomer dbCustomer = new DbCustomer();
        private static DbSession dbSession = new DbSession();
        private static SqlCommand dbCmd = null;

        private static Reservation createReservation(IDataReader dbReader)
        {
            Reservation reservation = new Reservation();

            reservation.ReservationId = Convert.ToInt32(dbReader["reservationId"].ToString());
            string fName = dbReader["custFName"].ToString();
            string lName = dbReader["custLName"].ToString();
            Customer customer = new Customer();
            customer = dbCustomer.getCustomerByName(fName, lName);
            reservation.Customer = customer;
            int sesionId = 0;
            sesionId = Convert.ToInt32(dbReader["sessionId"]);
            Session session = new Session();
            session = dbSession.getSessionById(sesionId);
            reservation.Session = session;
            reservation.NoOfSeats = Convert.ToInt32(dbReader["noOfSeats"].ToString());
            reservation.Price = Convert.ToInt32(dbReader["price"].ToString());
            reservation.Status = dbReader["status"].ToString();

            return reservation;
        }

        public int insertReservaion(Reservation reservation)
        {
            int result = -1;
            string sqlQuery = "INSERT INTO Reservation VALUES " +
                "('" + reservation.ReservationId + //get max
                "','" + reservation.Customer.CustomerFirstName + 
                "','" + reservation.Customer.CustomerLastName + 
                "','" + reservation.Session.SessionId + 
                "','" + reservation.NoOfSeats + 
                "','" + reservation.Price + "')";
            try
            {
                SqlCommand cmd = AccessDbSQLClient.GetDbCommand(sqlQuery);
                result = cmd.ExecuteNonQuery();
                AccessDbSQLClient.Close();
            }
            catch (SqlException) { }

            return result;
        }

        public List<int> insertReservedSeats(int reservationId, List<Seat> reservedSeats)
        {
            List<int> results = new List<int>();
            foreach (Seat seat in reservedSeats)
            {
                int result = -1;
                string sqlQuery = "INSERT INTO ReservedSeats VALUES " +
                    "('" + reservationId +
                    "','" + seat.SeatId + "')";
                    
                try
                {
                    SqlCommand cmd = AccessDbSQLClient.GetDbCommand(sqlQuery);
                    result = cmd.ExecuteNonQuery();
                    AccessDbSQLClient.Close();
                    results.Add(result);
                }
                catch (SqlException) { }
            }
            return results;
        }

        public Reservation getReservationById(int reservationId)
        {
            string sqlQuery = "SELECT * FROM Reservation WHERE reservationId = '" + reservationId + "'";
            dbCmd = AccessDbSQLClient.GetDbCommand(sqlQuery);

            IDataReader dbReader;
            dbReader = dbCmd.ExecuteReader();

            Reservation reservation = new Reservation();

            if (dbReader.Read())
            {
                reservation = createReservation(dbReader);
            }
            else
            {
                reservation = null;
            }

            return reservation;
        }

        public List<Reservation> getAllReservations()
        {
            List<Reservation> returnList = new List<Reservation>();
            string sqlQuery = "SELECT * FROM Reservation";
            dbCmd = AccessDbSQLClient.GetDbCommand(sqlQuery);

            IDataReader dbReader;
            dbReader = dbCmd.ExecuteReader();

            Reservation reservation = new Reservation();
            while (dbReader.Read())
            {
                reservation = createReservation(dbReader);
                returnList.Add(reservation);
            }

            return returnList;
        }

        public int updateReservation(Reservation reservation)
        {
            int result = -1;

            string sqlQuery = "UPDATE Reservation SET " +
                "custFName='" + reservation.Customer.CustomerFirstName + "', " +
                "custLName='" + reservation.Customer.CustomerLastName + "', " +
                "sessionId='" + reservation.Session.SessionId + "', " +
                "noOfSeats='" + reservation.NoOfSeats + "', " +
                "price='" + reservation.Price + "', " +
                "status='" + reservation.Status + "' WHERE " +
                "reservationId='" + reservation.ReservationId + "'";

            try
            {
                SqlCommand cmd = AccessDbSQLClient.GetDbCommand(sqlQuery);
                result = cmd.ExecuteNonQuery();
                AccessDbSQLClient.Close();
            }
            catch (SqlException)
            { }

            return result;
        }      
    }
}

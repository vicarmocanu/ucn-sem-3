﻿using Cinema.ModelLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.DBLayer
{
    class DbSeat: ISeat
    {
        private static SqlCommand dbCmd = null;
        private static DbRoom dbRoom = new DbRoom();

        //build a seat object based on the db reader
        private static Seat createSeat(IDataReader dbReader)
        {
            Seat seat = new Seat();   

            seat.SeatId = Convert.ToInt32(dbReader["seatId"].ToString());
            seat.SeatNumber = Convert.ToInt32(dbReader["seatNumber"].ToString());
            seat.Room = dbRoom.getRoomByNumber(Convert.ToInt32(dbReader["roomNumber"].ToString()));
            seat.RowNumber = Convert.ToInt32(Convert.ToInt32(dbReader["rowNumber"].ToString()));
            seat.Status = "E";

            return seat;
        }

        //insert the seats for a room
        public List<int> insertSeatMatrix(int rows, int columns, int roomNumber)
        {
            List<int> results = new List<int>();

            string attrib = "seatId";
            string table = "Seat";
            int max = 0;
            int id = 0;

            Room room = new Room();
            room = dbRoom.getRoomByNumber(roomNumber);
            int requiredNoOfSeats = room.NumberOfSeats;

            int noOfSeats = rows * columns;

            if (noOfSeats == requiredNoOfSeats)
            {
                for (int i = 1; i <= rows; i++)
                {
                    for (int j = 1; j <= columns; j++)
                    {
                        int result = -1;

                        max = GetMax.getMax(attrib, table);
                        id = max + 1;

                        int seatNumber = (i * columns) - (columns - j);
                        string sqlQuery = "INSERT INTO Seat VALUES " +
                            "('" + id +
                            "','" + seatNumber +
                            "','" + roomNumber +
                            "','" + i + "')";
                        try
                        {
                            SqlCommand cmd = AccessDbSQLClient.GetDbCommand(sqlQuery);
                            result = cmd.ExecuteNonQuery();
                            AccessDbSQLClient.Close();
                        }
                        catch (SqlException)
                        { }

                        results.Add(result);
                    }
                }
            }
            else { }
            return results;
        }

        //insert a seat
        public int insertSeat(Seat seat)
        {
            int result = -1;

            string attrib = "seatId";
            string table = "Seat";
            int max = GetMax.getMax(attrib, table);
            int id = max + 1;

            string sqlQuery = "INSERT INTO Seat VALUES " +
                "('" + id +
                "','" + seat.SeatNumber +
                "','" + seat.Room.RoomNumber +
                "','" + seat.RowNumber + "')";            
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

        //get all seats from a room
        public List<Seat> getRoomSeats(int roomNumber)
        {
            List<Seat> returnList = new List<Seat>();

            string sqlQuery = "SELECT * FROM Seat WHERE roomNumber= '" + roomNumber + "'";
            dbCmd = AccessDbSQLClient.GetDbCommand(sqlQuery);
            IDataReader dbReader;
            dbReader = dbCmd.ExecuteReader();

            Seat seat = new Seat();

            while (dbReader.Read())
            {
                seat = createSeat(dbReader);
                returnList.Add(seat);
            }

            AccessDbSQLClient.Close();

            return returnList;
        }

        //get all seats
        public List<Seat> getSeats()
        {
            List<Seat> returnList = new List<Seat>();

            string sqlQuery = "SELECT * FROM Seat";
            dbCmd = AccessDbSQLClient.GetDbCommand(sqlQuery);
            IDataReader dbReader;
            dbReader = dbCmd.ExecuteReader();

            Seat seat = new Seat();

            while (dbReader.Read())
            {
                seat = createSeat(dbReader);
                returnList.Add(seat);
            }

            AccessDbSQLClient.Close();

            return returnList;
        }

        //get a seat by its id
        public Seat getSeatById(int id)
        {
            string sqlQuery = "SELECT * FROM Seat WHERE seatId= '" + id + "'";
            dbCmd = AccessDbSQLClient.GetDbCommand(sqlQuery);
            IDataReader dbReader;
            dbReader = dbCmd.ExecuteReader();

            Seat seat = new Seat();

            if (dbReader.Read())
            {
                seat = createSeat(dbReader);
            }
            else
            {
                seat = null;
            }

            AccessDbSQLClient.Close();

            return seat;
        }

        //update a seat  - id
        public int updateSeat(Seat seat)
        {
            int result = -1;

            string sqlQuery = "UPDATE Seat SET " +
                "seatNumber='" + seat.SeatNumber + "', " +
                "roomNumber='" + seat.Room.RoomNumber + "', " +
                "rowNumber='" + seat.RowNumber + "' " +
                "WHERE seatId='" + seat.SeatId + "'";
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

        //delete a seat - id
        public int deleteSeat(int id)
        {
            int result = -1;
            String sqlQuery = "DELETE FROM Seat WHERE seatId= '" + id + "'";
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

        //delete the seats from a room
        public int deleteRoomSeats(int roomNumber)
        {
            int result = -1;
            String sqlQuery = "DELETE FROM Seat WHERE roomNumber= '" + roomNumber + "'";
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

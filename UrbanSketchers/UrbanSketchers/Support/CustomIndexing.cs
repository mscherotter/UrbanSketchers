// <copyright file="CustomIndexing.cs" company="Michael S. Scherotter">
// Copyright (c) 2012 Michael S. Scherotter All Rights Reserved
// </copyright>
// <author>James McCaffrey</author>
// <email>jammc@microsoft.com</email>
// <date>2012-11-157</date>
// <summary>Code from MSDN Magazine, June 2012, Custom Indexing for Latitude-Longitude Data
// http://archive.msdn.microsoft.com/mag201206indexing/Release/ProjectReleases.aspx?ReleaseId=5869
// </summary>

using System;

namespace UrbanSketchers.Support
{
    /// <summary>
    ///     Custom Map indexing
    /// </summary>
    public sealed class CustomIndexing
    {
        /// <summary>
        ///     The sector size
        /// </summary>
        public const double SectorSize = 0.5;

        /// <summary>
        ///     Gets the Latitude index from a latitude and fraction
        /// </summary>
        /// <param name="latitude">the latitude</param>
        /// <param name="fraction">the fraction</param>
        /// <returns>a latitude index</returns>
        public static int LatIndex(double latitude, double fraction)
        {
            latitude = Math.Round(latitude, 8);
            var lastIndex = (int) (180 * (1.0 / fraction) - 1);
            var firstLat = -90.0;

            if (latitude == -90.0) return 0;
            if (latitude == 90.0) return lastIndex;

            var lo = 0;
            var hi = lastIndex;
            var mid = (lo + hi) / 2;
            var ct = 0; // to prevent infinite loop

            while (ct < 10000)
            {
                ++ct;
                var left = firstLat + fraction * mid; // left part interval
                left = Math.Round(left, 8);
                var right = left + fraction; // right part interval
                right = Math.Round(right, 8);

                if (latitude >= left && latitude < right)
                    return mid;
                if (latitude < left)
                {
                    hi = mid - 1;
                    mid = (lo + hi) / 2;
                }
                else
                {
                    lo = mid + 1;
                    mid = (lo + hi) / 2;
                }
            }
            throw new Exception("LatIndex no return for input = " + latitude);
        }

        public static int LonIndex(double longitude, double fraction)
        {
            longitude = Math.Round(longitude, 8);
            var lastIndex = (int) (360 * (1.0 / fraction) - 1);
            var firstLon = -180.0;

            if (longitude == -180.0) return 0;
            if (longitude == 180.0) return lastIndex;

            var lo = 0;
            var hi = lastIndex;
            var mid = (lo + hi) / 2; // binary search
            var ct = 0; // to prevent runaway

            while (ct < 10000)
            {
                ++ct;
                var left = firstLon + fraction * mid; // left part of interval associated if mid is an index
                left = Math.Round(left, 8);
                var right = left + fraction; // right part of interval
                right = Math.Round(right, 8);

                ////Console.WriteLine("[" + left.ToString("F1") + " " + right.ToString("F1") + ")");

                if (longitude >= left && longitude < right)
                    return mid;
                if (longitude < left)
                {
                    hi = mid - 1;
                    mid = (lo + hi) / 2;
                }
                else
                {
                    lo = mid + 1;
                    mid = (lo + hi) / 2;
                }
            } // while

            throw new Exception("LonIndex never returned a value for input = " + longitude);
        }

        /// <summary>
        ///     Converts a lat/long to a sector
        /// </summary>
        /// <param name="latitude">the latitude</param>
        /// <param name="longitude">the longitude</param>
        /// <param name="fraction">the fraction</param>
        /// <returns>the sector</returns>
        public static int LatLonToSector(double latitude, double longitude, double fraction)
        {
            var latIndex = LatIndex(latitude, fraction); // row
            var lonIndex = LonIndex(longitude, fraction); // col
            return latIndex * 360 * (int) (1.0 / fraction) + lonIndex;
        }

        /// <summary>
        ///     Gets the distance from one location to another
        /// </summary>
        /// <param name="lat1">the latitude 1</param>
        /// <param name="lon1">the longitude 1</param>
        /// <param name="lat2">the latitude 2</param>
        /// <param name="lon2">the longitude 2</param>
        /// <returns>distance in KM</returns>
        public static double Distance(double lat1, double lon1, double lat2, double lon2)
        {
            var r = 6371.0; // approx. radius of earth in km
            var lat1Radians = lat1 * Math.PI / 180.0;
            var lon1Radians = lon1 * Math.PI / 180.0;
            var lat2Radians = lat2 * Math.PI / 180.0;
            var lon2Radians = lon2 * Math.PI / 180.0;
            var d = r * Math.Acos(Math.Cos(lat1Radians) *
                                  Math.Cos(lat2Radians) *
                                  Math.Cos(lon2Radians - lon1Radians) +
                                  Math.Sin(lat1Radians) * Math.Sin(lat2Radians));
            return d;
        }

        /// <summary>
        ///     Converts a sector to a latitude
        /// </summary>
        /// <param name="sector">the sector</param>
        /// <param name="fraction">the fraction</param>
        /// <returns>a latitude value</returns>
        public static double SectorToLat(int sector, double fraction)
        {
            var divisor = 360 * (int) (1.0 / fraction);
            var row = sector / divisor;
            return -90.0 + fraction * row;
        }

        public static double SectorToLon(int sector, double fraction)
        {
            var divisor = 360 * (int) (1.0 / fraction);
            var col = sector % divisor;
            return -180.0 + fraction * col;
        }

        public static double Area(int sector, double fraction)
        {
            var lat1 = SectorToLat(sector, fraction);
            var lon1 = SectorToLon(sector, fraction);
            var lat2 = lat1 + fraction;
            var lon2 = lon1 + fraction;
            var width = Distance(lat1, lon1, lat1, lon2);
            var height = Distance(lat1, lon1, lat2, lon1);
            return width * height;
        }

        public static bool IsLeftEdge(int sector, double fraction)
        {
            var numColumns = (int) (1.0 / fraction) * 360;
            if (sector % numColumns == 0) return true;
            return false;
        }

        public static bool IsRightEdge(int sector, double fraction)
        {
            if (IsLeftEdge(sector + 1, fraction))
                return true;

            return false;
        }

        public static bool IsTopRow(int sector, double fraction)
        {
            var numColumns = (int) (1.0 / fraction) * 360;
            if (sector >= 0 && sector <= numColumns - 1)
                return true;

            return false;
        }

        /// <summary>
        ///     Is the sector in the bottom row
        /// </summary>
        /// <param name="sector">the sector</param>
        /// <param name="fraction">the fraction</param>
        /// <returns></returns>
        public static bool IsBottomRow(int sector, double fraction)
        {
            var numColumns = (int) (1.0 / fraction) * 360;
            var numRows = (int) (1.0 / fraction) * 180;
            var firstValueInLastRow = numColumns * (numRows - 1);
            var lastValueInLastRow = numColumns * numRows - 1;
            if (sector >= firstValueInLastRow && sector <= lastValueInLastRow)
                return true;
            return false;
        }

        /// <summary>
        ///     Gets the adjacent sectors
        /// </summary>
        /// <param name="sector">the sector</param>
        /// <param name="fraction">the fraction</param>
        /// <returns>the adjacent sectors as a array</returns>
        public static int[] AdjacentSectors(int sector, double fraction)
        {
            var result = new int[8]; // clockwise from upper-left
            var numCols = (int) (1.0 / fraction) * 360;
            var numRows = (int) (1.0 / fraction) * 180;

            var firstValueInLastRow = numCols * (numRows - 1);
            var lastValueInLastRow = numCols * numRows - 1;

            // general case
            if (IsLeftEdge(sector, fraction) == false &&
                IsRightEdge(sector, fraction) == false &&
                IsTopRow(sector, fraction) == false &&
                IsBottomRow(sector, fraction) == false)
            {
                result[0] = sector - numCols - 1;
                result[1] = sector - numCols;
                result[2] = sector - numCols + 1;
                result[3] = sector - 1;
                result[4] = sector + 1;
                result[5] = sector + numCols - 1;
                result[6] = sector + numCols;
                result[7] = sector + numCols + 1;
                return result;
            }

            // top row except for left and right edges (which are corners)
            if (IsTopRow(sector, fraction) && IsLeftEdge(sector, fraction) == false &&
                IsRightEdge(sector, fraction) == false)
            {
                result[0] = firstValueInLastRow + sector - 1;
                result[1] = firstValueInLastRow + sector;
                result[2] = firstValueInLastRow + sector + 1;
                result[3] = sector - 1;
                result[4] = sector + 1;
                result[5] = sector + numCols - 1;
                result[6] = sector + numCols;
                result[7] = sector + numCols + 1;
                return result;
            }

            // bottom row except for edges (which are corners)
            if (IsBottomRow(sector, fraction) && IsLeftEdge(sector, fraction) == false &&
                IsRightEdge(sector, fraction) == false)
            {
                result[0] = sector - numCols - 1;
                result[1] = sector - numCols;
                result[2] = sector - numCols + 1;
                result[3] = sector - 1;
                result[4] = sector + 1;
                result[5] = sector - firstValueInLastRow - 1;
                result[6] = sector - firstValueInLastRow;
                result[7] = sector - firstValueInLastRow + 1;
                return result;
            }

            // left edge except for corners. tricky ones are left, upper left, and lower left
            if (IsLeftEdge(sector, fraction) && IsTopRow(sector, fraction) == false &&
                IsBottomRow(sector, fraction) == false)
            {
                result[0] = sector - 1;
                result[1] = sector - numCols;
                result[2] = sector - numCols + 1;
                result[3] = sector - 1 + numCols;
                result[4] = sector + 1;
                result[5] = sector - 1 + numCols + numCols;
                result[6] = sector + numCols;
                result[7] = sector + 1 + numCols;
                return result;
            }

            // right edge except for corners. tricky ones are right, upper right, lower right
            if (IsRightEdge(sector, fraction) && IsTopRow(sector, fraction) == false &&
                IsBottomRow(sector, fraction) == false)
            {
                result[0] = sector - numCols - 1;
                result[1] = sector - numCols;
                result[2] = sector + 1 - numCols - numCols;
                result[3] = sector - 1;
                result[4] = sector - numCols + 1;
                result[5] = sector + numCols - 1;
                result[6] = sector + numCols;
                result[7] = sector + 1;
                return result;
            }

            // upper left corner (always sector 0)
            if (sector == 0)
            {
                result[0] = numRows * numCols - 1;
                result[1] = firstValueInLastRow;
                result[2] = firstValueInLastRow + 1;
                result[3] = numCols - 1;
                result[4] = 1;
                result[5] = numCols + numCols - 1;
                result[6] = numCols;
                result[7] = numCols + 1;
                return result;
            }

            // upper right corner
            if (IsTopRow(sector, fraction) && IsRightEdge(sector, fraction))
            {
                result[0] = numCols * numRows - 2;
                result[1] = lastValueInLastRow;
                result[2] = firstValueInLastRow;
                result[3] = sector - 1;
                result[4] = 0;
                result[5] = sector + numCols - 1;
                result[6] = sector + numCols;
                result[7] = numCols;
                return result;
            }

            // lower left corner (is firstValueInLastRow)
            if (IsBottomRow(sector, fraction) && IsLeftEdge(sector, fraction))
            {
                result[0] = lastValueInLastRow - numCols;
                result[1] = sector - numCols;
                result[2] = sector - numCols + 1;
                result[3] = lastValueInLastRow;
                result[4] = sector + 1;
                result[5] = numCols - 1;
                result[6] = 0;
                result[7] = 1;
                return result;
            }

            // lower right corner (is lastValueInLastRow)
            if (IsBottomRow(sector, fraction) && IsRightEdge(sector, fraction))
            {
                result[0] = sector - numCols - 1;
                result[1] = sector - numCols;
                result[2] = firstValueInLastRow - numCols;
                result[3] = sector - 1;
                result[4] = firstValueInLastRow;
                result[5] = numCols - 2;
                result[6] = numCols - 1;
                result[7] = 0;
                return result;
            }

            // should never get here
            throw new Exception("Unexpected logic path in AdjacentSectors");
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Day3
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<string[]> linesReadingTask = System.IO.File.ReadAllLinesAsync("..\\..\\..\\input.txt");

            int[,] fabric = new int[1000, 1000];

            var lines = linesReadingTask.Result;

            List<Claim> claims = new List<Claim>();

            foreach (string claim in lines)
            {
                claims.Add(ClaimFactory.CreateClaim(claim));
            }

            ReserveClaimsOnFabric(fabric, claims);

            // Part1
            Console.WriteLine($"Number of fabric parts with conflicting claims: {CountConflictingFabricParts(fabric)}");

            // Part2

            List<Claim> claimsWithoutConflictingFabricParts = GetClaimsWithoutConflictingParts(fabric, claims);

            Console.Write("Claim id(s) without conflicting claims: ");

            foreach (var claim in claimsWithoutConflictingFabricParts)
            {
                Console.Write(claim.Id + " ");
            }

            Console.ReadLine();
        }

        private static void ReserveClaimsOnFabric(int[,] fabric, List<Claim> claims)
        {
            foreach (Claim claim in claims)
            {
                for (int i = 0; i < claim.LengthX; i++)
                {
                    for (int j = 0; j < claim.LengthY; j++)
                    {
                        fabric[claim.StartingPointX + i, claim.StartingPointY + j]++;
                    }
                }
            }
        }

        private static int CountConflictingFabricParts(int[,] fabric)
        {
            int fabricPartsWithConflictingClaims = 0;
            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    if (fabric[i, j] > 1)
                    {
                        fabricPartsWithConflictingClaims++;
                    }
                }
            }

            return fabricPartsWithConflictingClaims;
        }

        private static List<Claim> GetClaimsWithoutConflictingParts(int[,] fabric, List<Claim> claims)
        {
            var claimsWithoutConflictingParts = new List<Claim>();

            foreach (Claim claim in claims)
            {
                bool conflictFound = false;
                for (int i = 0; i < claim.LengthX; i++)
                {
                    for (int j = 0; j < claim.LengthY; j++)
                    {
                        if (fabric[claim.StartingPointX + i, claim.StartingPointY + j] > 1)
                        {
                            conflictFound = true;
                            break;
                        }
                    }
                }

                if (!conflictFound)
                {
                    claimsWithoutConflictingParts.Add(claim);
                }
            }

            return claimsWithoutConflictingParts;
        }
    }

    class Claim
    {
        public Claim(int id, int startingPointX, int startingPointY, int fabricClaimLengthX, int fabricClaimLengthY)
        {
            Id = id;
            StartingPointX = startingPointX;
            StartingPointY = startingPointY;
            LengthX = fabricClaimLengthX;
            LengthY = fabricClaimLengthY;
        }

        public int Id { get; }
        public int StartingPointX { get; }
        public int StartingPointY { get; }
        public int LengthX { get; }
        public int LengthY { get; }
    }

    class ClaimFactory
    {
        public static Claim CreateClaim(string stringFormat)
        {
            string[] claimParts = stringFormat.Split(' ');

            int claimId = int.Parse(claimParts[0].Substring(1));

            // remove ':'
            string[] startingPoint = claimParts[2].Remove(claimParts[2].Length - 1).Split(',');
            int fabricClaimStartingPointX = int.Parse(startingPoint[0]);
            int fabricClaimStartingPointY = int.Parse(startingPoint[1]);

            string[] length = claimParts[3].Split('x');
            int fabricClaimLengthX = int.Parse(length[0]);
            int fabricClaimLengthY = int.Parse(length[1]);

            return new Claim(claimId, fabricClaimStartingPointX, fabricClaimStartingPointY, fabricClaimLengthX, fabricClaimLengthY);
        }
    }

}

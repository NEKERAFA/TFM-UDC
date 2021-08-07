﻿using ColossalFramework.Math;
using IndustryLP.Utils.Enums;
using IndustryLP.Utils.Wrappers;
using System.Collections.Generic;
using UnityEngine;

namespace IndustryLP.DistributionDefinition
{
    /// <summary>
    /// This class represents a distribution result
    /// </summary>
    public abstract class DistributionInfo
    {
        /// <summary>
        /// The segments that made the road
        /// </summary>
        public List<Bezier3> Road { get; set; }

        /// <summary>
        /// The buildable cells in the distribution
        /// </summary>
        public List<ParcelWrapper> Cells { get; set; }

        public DistributionType Type { get; set; }

        /// <summary>
        /// Finds the adjacent neighbour following the distribution
        /// </summary>
        /// <param name="direction">The direction to look up</param>
        /// <param name="gridId">The id of the cell</param>
        /// <returns></returns>
        public abstract ParcelWrapper GetNext(CellNeighbour direction, ushort gridId);

        /// <summary>
        /// Finds the closest cell to that point, or null if all the points are far that the position to search
        /// </summary>
        /// <param name="position">The position to search</param>
        /// <param name="limit">The closest distance to find cells</param>
        /// <returns></returns>
        public abstract ParcelWrapper FindCell(Vector3 position, double? limit);

        public abstract ParcelWrapper FindById(ushort gridId);
    }
}

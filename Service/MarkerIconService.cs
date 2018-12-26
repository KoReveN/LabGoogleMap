using Domain.Entities;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public interface IMarkerIconService
    {
        IEnumerable<MarkerIcon> GetMarkerIcons();
    }


    public class MarkerIconService : IMarkerIconService
    {
        private readonly IMarkerIconRepository markerIconRepository;

        public MarkerIconService (IMarkerIconRepository markerIconRepository)
        {
            this.markerIconRepository = markerIconRepository;
        }

        public IEnumerable<MarkerIcon> GetMarkerIcons()
        {
            return markerIconRepository.GetAll();
           // return db.MarkerIcons;
        }
    }
}

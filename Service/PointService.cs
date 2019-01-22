using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{

    public interface IPointService
    {

    }

    class PointService : IPointService
    {
        private readonly IPointRepository pointRepository;

        public PointService(IPointRepository pointRepository)
        {
            this.pointRepository = pointRepository;
        }
    }
}

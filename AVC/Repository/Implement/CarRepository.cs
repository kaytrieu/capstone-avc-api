﻿using AVC.Models;
using AVC.Repository.Interface;

namespace AVC.GenericRepository.Implement
{
    public class CarRepository : BaseRepository<Car>, ICarRepository
    {
        public CarRepository(AVCContext dbContext) : base(dbContext)
        {
        }

    }
}

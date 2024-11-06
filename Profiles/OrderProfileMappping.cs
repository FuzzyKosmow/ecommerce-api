using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ecommerce_api.DTO.Order;
using ecommerce_api.Models;

namespace ecommerce_api.Profiles
{   /// <summary>
/// Used to map only update DTO to Model
/// </summary>
    public class OrderProfileMappping : Profile
    {
        public OrderProfileMappping()
        {
            //Change value that is not null
            CreateMap<UpdateOrderDTO, Order>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}
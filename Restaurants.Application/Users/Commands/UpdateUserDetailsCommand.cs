using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Users.Commands
{
    public class UpdateUserDetailsCommand : IRequest
    {

        public DateOnly DateOfBirth { get; set; }
        public string? Nationality { get; set; }
    }
}

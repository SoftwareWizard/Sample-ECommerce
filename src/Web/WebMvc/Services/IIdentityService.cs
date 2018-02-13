﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ShoesOnContainers.Web.WebMvc.Services
{
    public interface IIdentityService<out T>
    {
        T Get(IPrincipal principal);
    }
}

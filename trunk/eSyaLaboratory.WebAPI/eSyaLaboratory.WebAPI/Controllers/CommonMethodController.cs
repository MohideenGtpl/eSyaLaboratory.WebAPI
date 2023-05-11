﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eSyaLaboratory.DL.Repository;
using eSyaLaboratory.DO;
using eSyaLaboratory.IF;
using Microsoft.AspNetCore.Mvc;

namespace eSyaLaboratory.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommonMethodController : ControllerBase
    {
        private readonly ICommonMethodRepository _CommonMethodRepository;
        public CommonMethodController(ICommonMethodRepository commonmethodRepository)
        {
            _CommonMethodRepository = commonmethodRepository;
        }
        public async Task<IActionResult> GetBusinessKey()
        {
            var ac = await _CommonMethodRepository.GetBusinessKey();
            return Ok(ac);
        }
        public async Task<IActionResult> GetApplicationCodesByCodeType(int codetype)
        {
            var ac = await _CommonMethodRepository.GetApplicationCodesByCodeType(codetype);
            return Ok(ac);
        }
        public async Task<IActionResult> GetServiceClasses(int servicetype)
        {
            var ac = await _CommonMethodRepository.GetServiceClasses(servicetype);
            return Ok(ac);
        }
        public async Task<IActionResult> GetLabServicesByBKey(int businessKey)
        {
            var ac = await _CommonMethodRepository.GetLabServicesByBKey(businessKey);
            return Ok(ac);
        }
    }
}
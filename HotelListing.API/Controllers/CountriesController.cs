﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using AutoMapper;
using HotelListing.API.Entity;
using HotelListing.API.Models.Country;
using HotelListing.API.Repository.IRepostitory;
using Microsoft.AspNetCore.Authorization;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CountriesController : ControllerBase
    {
        private readonly ICountriesRepository _countriesRepository;
        private readonly IMapper _mapper;
        public CountriesController(ICountriesRepository countriesRepository, IMapper mapper)
        {
            _countriesRepository = countriesRepository;
            _mapper = mapper;

        }

        // GET: api/Countries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
            var countries = await _countriesRepository.GetAllAsync();
            var records  = _mapper.Map<List<GetCountryDto>>(countries);
            return Ok(records);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            var country = await _countriesRepository.GetDetails(id);

            if (country == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CountryDto>(country));
        }

        // PUT: api/Countries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
        {
            if (id != updateCountryDto.Id)
            {
                return BadRequest();
            }
            var country = await _countriesRepository.GetAsync(id);
            if (country == null)
            {
                return NotFound("Record with the give Id not found");
            }
            _mapper.Map(updateCountryDto, country);
            //_context.Entry(country).State = EntityState.Modified;

            try
            {
                await _countriesRepository.UpdateAsync(country);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CountryExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Countries
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createConutryDto)
        {
         
            var country = _mapper.Map<Country>(createConutryDto);
            await _countriesRepository.AddAsync(country);  
            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _countriesRepository.GetAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            await _countriesRepository.DeleteAsync(id);
            return NoContent();
        }

        private async Task<bool> CountryExistsAsync(int id)
        {
            return await _countriesRepository.Exists(id);
        }
    }
}

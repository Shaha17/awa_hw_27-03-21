using System.Threading;
using System.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using DZ_27_03_21.Models;
using System.Data.SqlClient;
using Dapper;

namespace DZ_27_03_21.Controllers
{
	public class PersonController : Controller
	{
		private readonly ILogger<PersonController> _logger;
		private readonly string _conStr;
		private readonly IConfiguration _configuration;

		public PersonController(ILogger<PersonController> logger, IConfiguration configuration)
		{
			_configuration = configuration;
			_logger = logger;
			_conStr = _configuration.GetConnectionString("Default");
		}
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var persons = new List<Person>();
			try
			{
				using (IDbConnection conn = new SqlConnection(_conStr))
				{
					persons = (await conn.QueryAsync<Person>("SELECT * FROM Persons")).ToList();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			return View(persons);
		}
		[HttpGet]
		public async Task<IActionResult> Delete(int id)
		{
			if (id <= 0)
			{
				return RedirectToAction("Index");
			}
			Person per = new Person();
			try
			{
				using (IDbConnection conn = new SqlConnection(_conStr))
				{
					per = await conn.QueryFirstOrDefaultAsync<Person>($"SELECT * FROM Persons WHERE Id = {id}");
				}
				if (per.Id <= 0)
				{
					return RedirectToAction("Index");
				}
				using (IDbConnection conn = new SqlConnection(_conStr))
				{
					await conn.ExecuteAsync($"DELETE FROM Persons WHERE Id = {per.Id}");
				}
			}
			catch (Exception ex)
			{
				System.Console.WriteLine(ex.Message);
				return RedirectToAction("Index");

			}

			return RedirectToAction("Index");
		}
		[HttpPost]
		public async Task<IActionResult> Create(Person model)
		{
			if (model == null)
			{
				return RedirectToAction("Index");
			}
			try
			{
				using (IDbConnection conn = new SqlConnection(_conStr))
				{
					await conn.ExecuteAsync("INSERT INTO Persons(FirstName,LastName,MiddleName) VALUES(@FirstName,@LastName,@MiddleName) ", model);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return RedirectToAction("Index");
		}
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}
		[HttpGet]
		public async Task<IActionResult> FindById(int id)
		{
			if (id <= 0)
			{
				return RedirectToAction("Index", "Home");
			}
			var lst = new List<Person>();
			try
			{
				using (IDbConnection conn = new SqlConnection(_conStr))
				{
					lst = (await conn.QueryAsync<Person>($"SELECT * FROM Persons WHERE Id = {id}")).ToList();
				}
			}
			catch (Exception ex)
			{
				System.Console.WriteLine(ex.Message);
			}
			return View("Index", lst);
		}
		[HttpGet]
		public async Task<IActionResult> FindByFio(string fio)
		{
			if (fio == null || fio == "")
			{
				return RedirectToAction("Index", "Home");
			}
			var lst = new List<Person>();
			try
			{
				using (IDbConnection conn = new SqlConnection(_conStr))
				{
					lst = (await conn.QueryAsync<Person>($"SELECT * FROM Persons WHERE (LastName+' '+FirstName+' '+MiddleName) LIKE '%{fio}%' ")).ToList();
				}
			}
			catch (Exception ex)
			{
				System.Console.WriteLine(ex.Message);
			}
			return View("Index", lst);
		}



	}
}
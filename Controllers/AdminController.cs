using Microsoft.Ajax.Utilities;
using MVC_project.Dal;
using MVC_project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;



namespace MVC_project.Controllers
{
    public class AdminController : Controller
    {

        CinemaEntities db = new CinemaEntities();
        public ActionResult AdminPage()//open view Adminpage
        {
   
            return View();
        }
      

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        [HttpGet]
        public ActionResult MovieAddingRemove()//Sending to view MovieAddingRemove list of genres in db and list of movies that registered
        {

            var namesGeners = (from x in db.GENRES
                               select x.NAME).ToList<String>();

            ViewBag.genres = new SelectList(namesGeners, "NAME");

            var namesMovies = (from x in db.MOVIES
                               select x.NAME).ToList<String>();
            ViewBag.names = new SelectList(namesMovies, "NAME");

            return View();
        }

        [HttpPost]
        public ActionResult MovieAddingRemove(MOVIES movie)//Adding the movie from the model to db movie tbl
        {
            string fileName = Path.GetFileNameWithoutExtension(movie.PictureFile.FileName);
            string extension = Path.GetExtension(movie.PictureFile.FileName);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            movie.PicturePath = "~/Image/" + fileName;
            fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
            movie.PictureFile.SaveAs(fileName);
            

            db.MOVIES.Add(movie);
            db.SaveChanges();

            


            return RedirectToAction("MovieAddingRemove", "Admin");
        }


        [HttpPost]
        public ActionResult MovieRemove(MOVIES moviex)//Removing movie from the movie tbl in db
        {
            /*var y = (from x in db.MOVIES
                     where x.ID == MOVIE.id1
                     select x.NAME).Single().ToString();    select specific name*/


            MOVIES movie = db.MOVIES.Where(x => x.NAME == moviex.NAME).Single();
            db.MOVIES.Remove(movie);
            db.SaveChanges();

            return RedirectToAction("MovieAddingRemove", "Admin");
        }

        [HttpGet]
        public ActionResult ManagingMovieHalls()//Managing the Movies in Halls, sending to view list of movies and list of halls.
        {
            var movies = (from x in db.MOVIES
                          select x.NAME).ToList<String>();

            ViewBag.movies = new SelectList(movies, "NAME");

            var halls = (from x in db.HALL
                         select x.HallNumber).ToList<int>();
            ViewBag.halls = new SelectList(halls, "ID");

            return View();

        }

        [HttpPost]
        public ActionResult ManagingMovieHalls(SCHEDULE sche)//get from the view movie,hall and send to new view list of free times to create schdule. 
        {

            List<Time> CatchTimes = db.SCHEDULE.Where(x => x.HallNumber == sche.HallNumber && x.Date == sche.Date).Select(x => new Time() { FromTime = x.FromTime, ToTime = x.ToTime }).ToList<Time>();
            List<Time> freeTimes = CreateListOfTimes(CatchTimes,sche);
            SaveSchedule(sche,freeTimes);
                
            List<string> freeTimesString = new List<string>();
            for (int i = 0; i < freeTimes.Count(); i++)
                freeTimesString.Add(freeTimes.ElementAt(i).FromTime.ToString()+"-"+freeTimes.ElementAt(i).ToTime.ToString());
            
            ViewBag.freeTimes=new SelectList(freeTimesString, "ID");
            return View("ManagingHour", sche);
        }

        [HttpPost]
        public ActionResult ManagingHour(SCHEDULE sche)
        {

            sche = LoadSchedule(sche);
            db.SCHEDULE.Add(sche);
            db.SaveChanges();


            return RedirectToAction("ManagingMovieHalls","Admin");
        }



        [HttpGet]
        public ActionResult ManagingNumberOfSeats()
        {
            var halls = (from x in db.HALL
                         select x.HallNumber).ToList<int>();
            ViewBag.halls = new SelectList(halls, "ID");

            return View();
        }
        [HttpPost]
        public ActionResult ManagingNumberOfSeats(HALL hall)
        {
            //var itemsToDelete = db.SEAT.Where(x => x.HallNumber==hall.HallNumber);
            //db.SEAT.RemoveRange(itemsToDelete);
            
            HALL hal_ = db.HALL.Where(x => x.HallNumber == hall.HallNumber).Single();
            db.HALL.Remove(hal_);
            db.HALL.Add(hall);
            db.SaveChanges();

            return RedirectToAction("ManagingNumberOfSeats", "Admin");
        }

        [HttpGet]
        public ActionResult ManagingPrices()
        {

            var movies = (from x in db.MOVIES
                          select x.NAME).ToList<String>();

            ViewBag.movies = new SelectList(movies, "NAME");
            ViewBag.flag = false;
            return View();
        }
        [HttpPost]
        public ActionResult ManagingPrices(MOVIES m)
        {
            MOVIES movie = db.MOVIES.Where(x => x.NAME == m.NAME).Single();
            var movies = (from x in db.MOVIES
                          select x.NAME).ToList<String>();
            List<int> sales = new List<int> { 0,10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
            ViewBag.movies = new SelectList(movies, "NAME");
            ViewBag.sales = new SelectList(sales, "NAME");
            ViewBag.flag = true;
            TempData["movie"] = movie;
            return View(movie);
        }
        
        public ActionResult ManagingSale(MOVIES m)  
        {
            
            MOVIES movie = (MOVIES)TempData["movie"];
            
            var result = db.MOVIES.SingleOrDefault(b => b.NAME == movie.NAME);
            if (result != null)
            {
                result.Sale = (int)m.newSale;
                db.SaveChanges();
            }
            /*if (m.Sale == 0)

                  result.PRICE = (1%(1-movie.Sale)) * movie.PRICE;
              else
                  result.PRICE = (1 - (m.Sale * 0.01)) * movie.PRICE;*/

            return RedirectToAction("ManagingPrices", "Admin");
        }

        public SCHEDULE LoadSchedule(SCHEDULE sche)
        {

            sche.FromTime = ScheduleBuild.freeTimes.ElementAt(sche.Id).FromTime;
            sche.ToTime = ScheduleBuild.freeTimes.ElementAt(sche.Id).ToTime;
            sche.Movie = ScheduleBuild.Movie;
            sche.HallNumber=ScheduleBuild.HallNumber;
            sche.Date=ScheduleBuild.Date;
            sche.Id=ScheduleBuild.Id;
            
            return sche;
        }

        public void SaveSchedule(SCHEDULE sche ,List<Time> freeTimes)
        {
            ScheduleBuild.Movie = sche.Movie;
            ScheduleBuild.HallNumber = sche.HallNumber;
            ScheduleBuild.Date = sche.Date;
            ScheduleBuild.Id = sche.Id;
            ScheduleBuild.freeTimes = freeTimes;
        }

        public List<Time> CreateListOfTimes(List<Time> CatchTimes, SCHEDULE sche)
        {
            List<Time> freeTime = new List<Time>();
            bool free = true;
            int movieTimeMinutes = (from x in db.MOVIES
                             where x.NAME == sche.Movie
                             select x.AmountOfTime).Single();
            float movieTimeHour = CastMinutesToHours(movieTimeMinutes);  //cast from minutes to hours
            

            for (int i = 6; i < 24- movieTimeHour; i++)
            {
                for (int j = 0; j < CatchTimes.Count(); j++) {
                    if (CheckValidation(i, j, CatchTimes, movieTimeHour)) {
                        free = false;
                        j = CatchTimes.Count();
                    }
                }
                if (free == true) {
                    
                    freeTime.Add(new Time() { FromTime = new TimeSpan(i, 0, 0), ToTime = new TimeSpan(i + (int)movieTimeHour, movieTimeMinutes % 60, 0) });

                }
                free = true;
            }

            return freeTime;
        }

        public bool CheckValidation(int i,int j, List<Time> CatchTimes,float movieTimeHour)
        {

            return ((CatchTimes.ElementAt(j).FromTime.Hours <= i && CatchTimes.ElementAt(j).ToTime.Hours+ CastMinutesToHours(CatchTimes.ElementAt(j).ToTime.Minutes) > i)//catch exm: 13-15  new (14)-16
                || (CatchTimes.ElementAt(j).FromTime.Hours > i && CatchTimes.ElementAt(j).FromTime.Hours < i + movieTimeHour)//catch exm: 13-15  new 12-14
             
                || i + movieTimeHour > 24);



        }

        float CastMinutesToHours(float minutes)
        {
            return (minutes / 60) + ((minutes % 60) / 100);

        }






       

       
    }
}
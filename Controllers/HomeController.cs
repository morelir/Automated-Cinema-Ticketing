using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_project.Models;

namespace MVC_project.Controllers
{

    public class HomeController : Controller
    {
        CinemaEntities db = new CinemaEntities();
        [HttpGet]
        public ActionResult MovieGallery(USER user=null)//send to view MovieGallery VmModel that include list of movies and schdules
        {
            
            Session["user"] = user;
            var namesGeners = (from x in db.GENRES
                               select x.NAME).ToList<String>();
            Session["genres"] = new SelectList(namesGeners, "NAME");
            List<SCHEDULE> schdules = (from s in db.SCHEDULE select s).ToList<SCHEDULE>();
            List<MOVIES> movies = (from m in db.MOVIES select m).ToList<MOVIES>();
            
           /* for (int i=0; i<movies.Count(); i++)
            {
                movies.ElementAt(i).PRICE= movies.ElementAt(i).PRICE * (1 - (movies.ElementAt(i).Sale / 100));
            }*/
            Session["movies"] = movies;
            GalleryVM vm = new GalleryVM() { movies = movies,schdules=schdules, schedule = new SCHEDULE(), movie = new MOVIES() ,user=(USER) Session["user"] };

            return View(vm);
        }
        public ActionResult CurrentMovieGallery()//Action Result for return to view page that already been "MovieGallery"
        {
            var namesGeners = (from x in db.GENRES
                               select x.NAME).ToList<String>();
            Session["genres"] = new SelectList(namesGeners, "NAME");
            List<SCHEDULE> schdules = (from s in db.SCHEDULE select s).ToList<SCHEDULE>();
            List<MOVIES> movies = (from m in db.MOVIES select m).ToList<MOVIES>();
            Session["movies"] = movies;
            GalleryVM vm = new GalleryVM() { movies = movies, schdules = schdules, schedule = new SCHEDULE(), movie = new MOVIES(), user = (USER)Session["user"] };

            return View("MovieGallery",vm);
        }
        [HttpPost]
        public ActionResult MovieScheduleDate()//send to MovieScheduleDate view the list of Dates for specific movie 
        {
            string indexString = Request.Form["index"];
            int index = Int32.Parse(indexString);
            List<MOVIES> movies = (List<MOVIES>)Session["movies"];
            string name = movies.ElementAt(index).NAME;
            List<SCHEDULE> schdules = (from s in db.SCHEDULE where s.Movie==name select s).ToList<SCHEDULE>();
            Session["schdules"] = schdules;
            List<string> dates = new List<string>();
            foreach (SCHEDULE sch in schdules)
                dates.Add(sch.Date.ToShortDateString());
            Session["dates"]= new SelectList(dates, "NAME");
            ViewData["MovieName"] = schdules.ElementAt(0).Movie;
            return View();
        }
        public ActionResult CurrentMovieScheduleDate()//Action Result for return to view page that already been "MovieScheduleDate"
        {

            List<SCHEDULE> schdules = (List<SCHEDULE>)Session["schdules"];
            ViewData["MovieName"] = schdules.ElementAt(0).Movie;
            return View("MovieScheduleDate");
        }
        [HttpPost]
        public ActionResult MovieScheduleHour(GalleryVM vm)//send to MovieSchduleHour view the list of times for specific movie in specific time
        {
            List<SCHEDULE> schdules = (List<SCHEDULE>)Session["schdules"];
            ViewData["MovieName"] = schdules.ElementAt(0).Movie;
            List<string> times = new List<string>();
            int counter = 0;
            List<int> counters=new List<int>();
            foreach (SCHEDULE sch in schdules)//search the selected schdule
            {
                if (sch.Date == vm.schedule.Date)
                {
                    times.Add(sch.FromTime.ToString() + "-" + sch.ToTime.ToString());//add to list the hours of movie in specific date
                    counters.Add(counter);
                }
                counter++;
            }
            Session["DateCounter"] = counters;
            ViewData["hours"] = new SelectList(times, "ID");
            return View();
        }

        [HttpPost]
        public ActionResult MovieHall(GalleryVM vm)//send to MovieHall view, the List of seats that include occupied seats
        {
            List<SCHEDULE> schdules = (List<SCHEDULE>)Session["schdules"];
            ViewData["MovieName"] = schdules.ElementAt(0).Movie;
            List<int> counters = (List<int>)Session["DateCounter"];
            DateTime date = schdules.ElementAt(counters.ElementAt(vm.schedule.Id)).Date; //date index of the selected hour
            TimeSpan FromTime= schdules.ElementAt(counters.ElementAt(vm.schedule.Id)).FromTime; //from time
            TimeSpan ToTime = schdules.ElementAt(counters.ElementAt(vm.schedule.Id)).ToTime;    // to time
            int HallNumber= schdules.ElementAt(counters.ElementAt(vm.schedule.Id)).HallNumber;  //hall number
            List<Seat> caught = db.Orders.
                                Where(o => o.Date == date && o.FromTime == FromTime && o.ToTime == ToTime && o.HallNumber == HallNumber).
                                Select(o => new Seat() { line = o.Line, chair = o.Chair }).ToList<Seat>(); //caught seats in the hall
            Seats seats = new Seats { caught = caught };
            ViewData["Lines"] = db.HALL.Where(x => x.HallNumber == HallNumber).Select(x => x.Lines).Single(); //count lines in hall
            ViewData["ChairsInLine"] = db.HALL.Where(x => x.HallNumber == HallNumber).Select(x => x.ChairsInLine).Single();// count seats in each hall
            Orders order=new Orders { Date = date, FromTime = FromTime, ToTime = ToTime, HallNumber = HallNumber, Movie = schdules.ElementAt(0).Movie };
            Session["order"] = order;

            return View(seats);
            
        }
        [HttpPost]
        public ActionResult Purchase()//Create a viewModel for form in view of movie order
        {
            Orders order = (Orders)Session["order"];
            order.Line = Int32.Parse(Request.Form["line"]);
            order.Chair = Int32.Parse(Request.Form["chair"]);
            
            double priceWithoutSale= db.MOVIES.Where(x => x.NAME == order.Movie).Select(x => x.PRICE).Single();
            int sale= db.MOVIES.Where(x => x.NAME == order.Movie).Select(x => x.Sale).Single();
            double totalPrice = priceWithoutSale * (1 - (sale / 100));
            order.Price = totalPrice;
            Session["order"] = order;
            ViewData["date"] = order.Date.ToShortDateString();
            PurchaseVM vm = new PurchaseVM() { order = order, payment = new Payment() };
            return View(vm);
        }
        [HttpPost]
        public ActionResult PurchaseConfirm(PurchaseVM vm) //Confirm the purchase of movie order
        {
            Orders order = (Orders)Session["order"];
            order.FirstName = vm.order.FirstName;
            order.LastName = vm.order.LastName;
            order.PersonId = vm.order.PersonId;
            
            db.Orders.Add(order);
            db.SaveChanges();

            return RedirectToAction("CurrentMovieGallery");
        }
        
        public ActionResult PriceDecrese() //send to view sorted movies list by Decrease Price
        {
            List<MOVIES> movies = PriceDecreseSort();
            GalleryVM new_vm = new GalleryVM() { movies = movies, schedule = new SCHEDULE(), movie = new MOVIES(), user = (USER)Session["user"] };
            return View("MovieGallery", new_vm);
        }

        
        public ActionResult PriceIncrease() //send to view sorted movies list Increase Price
        {
            List<MOVIES> movies = PriceIncreaseSort();
            GalleryVM new_vm = new GalleryVM() { movies = movies, schedule = new SCHEDULE(), movie = new MOVIES(), user = (USER)Session["user"] };
            return View("MovieGallery", new_vm);
        }
        public ActionResult MostPopular() //return list of movies sorted by Increase price 
        {
            List<MOVIES> movies = (from m in db.MOVIES select m).ToList<MOVIES>();
            List<MOVIES> SortedMovies = movies.OrderBy(o => o.AGE_LIMITAION).ToList();
            GalleryVM new_vm = new GalleryVM() { movies = SortedMovies, schedule = new SCHEDULE(), movie = new MOVIES(), user = (USER)Session["user"] };
            return View("MovieGallery", new_vm);

        }

        public ActionResult Category() //return list of movies sorted by Increase price 
        {
            List<MOVIES> movies = (from m in db.MOVIES select m).ToList<MOVIES>();
            List<MOVIES> SortedMovies = movies.OrderByDescending(o => o.GENRE).ToList();
            GalleryVM new_vm = new GalleryVM() { movies = SortedMovies, schedule = new SCHEDULE(), movie = new MOVIES(), user = (USER)Session["user"] };
            return View("MovieGallery", new_vm);

        }

        public ActionResult SaleMovies()//send to view movies that have a sale
        {

            List<MOVIES> movies = SearchSaleMovies();
            GalleryVM new_vm = new GalleryVM() { movies = movies, schedule = new SCHEDULE(), movie = new MOVIES(), user = (USER)Session["user"] };

            return View("MovieGallery", new_vm);
        }

        [HttpPost]
        public ActionResult GalleryAdvancedSearch(GalleryVM vm) // send to view movies due to advances search
        {

            List<MOVIES> movies = SearchMovies(vm);
            GalleryVM new_vm = new GalleryVM() { movies = movies, schedule = new SCHEDULE(), movie = new MOVIES() ,user= (USER)Session["user"] };
            
            return View("MovieGallery",new_vm);
        }


        public List<MOVIES> PriceDecreseSort() //return list of movies sorted by Decrease price 
        {
            List<MOVIES> movies = (from m in db.MOVIES select m).ToList<MOVIES>();
            List<MOVIES> SortedMovies = movies.OrderBy(o => o.PRICE).ToList();
            return SortedMovies;

        }
        public List<MOVIES> PriceIncreaseSort() //return list of movies sorted by Increase price 
        {
            List<MOVIES> movies = (from m in db.MOVIES select m).ToList<MOVIES>();
            List<MOVIES> SortedMovies = movies.OrderByDescending(o => o.PRICE).ToList();
            return SortedMovies;

        }

        

        public List<MOVIES> SearchSaleMovies() //return list of movies that have a sale
        {
            List<MOVIES> movies = (from m in db.MOVIES where m.Sale > 0 select m).ToList<MOVIES>();

            return movies;
        }
        public List<MOVIES> SearchMovies(GalleryVM vm)   //search movies by (date,time,price,genre),return matching movies
        {
            List<SCHEDULE> schdules = (from s in db.SCHEDULE where s.Date == vm.movie.LASTDATE && s.FromTime >= vm.FromTime select s).ToList<SCHEDULE>();
            List<MOVIES> movies = (from m in db.MOVIES where m.GENRE == vm.movie.GENRE && m.PRICE <= vm.movie.PRICE select m).ToList<MOVIES>();
            bool exists = false;
            for (int i = 0; i < movies.Count();)
            {
                for (int j = 0; j < schdules.Count(); j++)
                {
                    if (movies.ElementAt(i).NAME == schdules.ElementAt(j).Movie)
                    {
                        exists = true;
                        j = schdules.Count();
                    }

                }
                if (exists == false)
                    movies.RemoveAt(i);
                else
                    i++;
                exists = false;
            }
            return movies;

        }
    }
}
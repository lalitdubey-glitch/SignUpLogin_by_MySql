using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Tls;
using SignUpLogin_by_MySql.Models;
using MySql.Data.MySqlClient;
using System.Data;
using RestSharp;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace SignUpLogin_by_MySql.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        DBLayer db;

        public HomeController(IWebHostEnvironment env, IConfiguration config)
        {
            db = new DBLayer(config);
            _env = env; 
        }


        public ActionResult Index()
        {
           ViewBag.User =  Request.Cookies["UEmail"];
            return View();
        }
         
        public ActionResult Dashboard()
        {
            if (Request.Cookies["UEmail"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.User = Request.Cookies["UEmail"];
                return View();
            } 
        }
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(SignUpModel s , int? id)
        {
            string Hashpss= null;
            if (!String.IsNullOrEmpty(s.pass)){
                Hashpss = BCrypt.Net.BCrypt.HashPassword(s.pass);
            }
            string action = "";
            if (id.HasValue)
            {
                action = "edit";
            }
            else
            {
                action = "add";
            }
                string pic = null;

            if (s.img != null)
            {
                string filename = Path.GetFileName(s.img.FileName);
                string path = Path.Combine(_env.WebRootPath,"UserImg",filename);
                using var stream = new FileStream(path, FileMode.Create);
                s.img.CopyTo(stream);
                pic = filename;
            }

            MySqlParameter msg = new MySqlParameter("p_res", MySqlDbType.VarChar, 255);
            msg.Direction = ParameterDirection.Output;
           

            int res = db.ExecuteQuery("sp_SignUp", new MySqlParameter[]
            {
                new MySqlParameter("p_action",action),
                new MySqlParameter("p_id",id!=null?id : DBNull.Value),
                new MySqlParameter("p_name",s.name),
                new MySqlParameter("p_email",s.email),
                new MySqlParameter("p_mob",s.mob!=null?s.mob:DBNull.Value),
                new MySqlParameter("p_pass",Hashpss!=null?Hashpss:DBNull.Value),
                new MySqlParameter("p_dob",s.dob !=null ? s.dob.Value.ToString("yyyy-MM-dd"):DBNull.Value),
                new MySqlParameter("p_img",pic!=null?pic:DBNull.Value),
                new MySqlParameter("p_gender",s.gender != null ? s.gender : DBNull.Value),
                new MySqlParameter("p_hobby",s.hobby != null ? string.Join(", ",s.hobby) : DBNull.Value),
                new MySqlParameter("p_profession",s.profession != null ? s.profession : DBNull.Value),
                new MySqlParameter("p_pincode",s.pincode != null ? s.pincode : DBNull.Value),
                new MySqlParameter("p_state",s.state != null ? s.state : DBNull.Value),
                new MySqlParameter("p_dist",s.dist != null ? s.dist : DBNull.Value),
                new MySqlParameter("p_vill",s.vill != null ? s.vill : DBNull.Value),
                msg
            });

            string resMsg = msg.Value.ToString();

            if (resMsg == "User Already Exists")
            {
                return Json(new{ exist = true});
            }
            return Json(new { success = true });
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel l)
        {
            
            MySqlParameter msg = new MySqlParameter("p_res", MySqlDbType.VarChar, 255);
            msg.Direction = ParameterDirection.Output;

            DataTable dt = db.table("sp_SignUp", new MySqlParameter[]
            {
                new MySqlParameter("p_action" , "LoginUser"),
                new MySqlParameter("p_email" , l.email), 
                new MySqlParameter("p_id", DBNull.Value),
                new MySqlParameter("p_name",DBNull.Value), 
                new MySqlParameter("p_pass",DBNull.Value), 
                new MySqlParameter("p_mob",DBNull.Value), 
                new MySqlParameter("p_dob",DBNull.Value),
                new MySqlParameter("p_img", DBNull.Value),
                new MySqlParameter("p_gender",DBNull.Value),
                new MySqlParameter("p_hobby",DBNull.Value),
                new MySqlParameter("p_profession",DBNull.Value),
                new MySqlParameter("p_pincode",DBNull.Value),
                new MySqlParameter("p_state",DBNull.Value  ),
                new MySqlParameter("p_dist",DBNull.Value),
                new MySqlParameter("p_vill",DBNull.Value),
                msg
            });

            if (dt.Rows.Count > 0) 
            {
                string p = dt.Rows[0]["pass"].ToString();

                bool isValid = BCrypt.Net.BCrypt.Verify(l.pass,p);
                if (isValid) 
                {
                    if (l.remeber)
                        {
                        Response.Cookies.Append("UEmail", l.email, new CookieOptions
                        {
                            Expires = DateTime.Now.AddDays(7)
                        });
                        Response.Cookies.Append("UId", dt.Rows[0]["id"].ToString(), new CookieOptions
                        {
                            Expires= DateTime.Now.AddDays(7)
                        });
                        return Json(new { success = true });

                    }
                    else
                    {
                        Response.Cookies.Append("UId", dt.Rows[0]["id"].ToString());
                        Response.Cookies.Append("UEmail", l.email);
                    }
                    return Json(new { success = true });
                } 
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult DeleteUser(int? id)
        {

            MySqlParameter msg = new MySqlParameter("p_res", MySqlDbType.VarChar, 255);
            msg.Direction = ParameterDirection.Output;

            if (id.HasValue)
            {
                db.ExecuteQuery("sp_SignUp", new MySqlParameter[]{
                new MySqlParameter("p_action" , "delete"),
                new MySqlParameter("p_id", id),
                new MySqlParameter("p_name",DBNull.Value),
                new MySqlParameter("p_email",DBNull.Value),
                new MySqlParameter("p_mob",DBNull.Value),
                new MySqlParameter("p_pass",DBNull.Value),
                new MySqlParameter("p_dob",DBNull.Value),
                new MySqlParameter("p_img", DBNull.Value),
                new MySqlParameter("p_gender",DBNull.Value),
                new MySqlParameter("p_hobby",DBNull.Value),
                new MySqlParameter("p_profession",DBNull.Value),
                new MySqlParameter("p_pincode",DBNull.Value),
                new MySqlParameter("p_state",DBNull.Value  ),
                new MySqlParameter("p_dist",DBNull.Value),
                new MySqlParameter("p_vill",DBNull.Value),
                msg
            });

            }
            string resMsg = msg.Value.ToString();

            if(resMsg == "Success")
            {
                return Content("Deleted" , "application/json");
            }
            else
            {
                return Json(new { success = false });
            }
        }
        public ActionResult GetAllUsersData()
        {
            MySqlParameter msg = new MySqlParameter("p_res", MySqlDbType.VarChar, 255);
            msg.Direction = ParameterDirection.Output;

            DataTable dt = db.table("sp_SignUp", new MySqlParameter[]{
                new MySqlParameter("p_action" , "SelectAll"),
                new MySqlParameter("p_id", DBNull.Value),
                new MySqlParameter("p_name",DBNull.Value),
                new MySqlParameter("p_email",DBNull.Value),
                new MySqlParameter("p_mob",DBNull.Value),
                new MySqlParameter("p_pass",DBNull.Value),
                new MySqlParameter("p_dob",DBNull.Value),
                new MySqlParameter("p_img", DBNull.Value),
                new MySqlParameter("p_gender",DBNull.Value),
                new MySqlParameter("p_hobby",DBNull.Value),
                new MySqlParameter("p_profession",DBNull.Value),
                new MySqlParameter("p_pincode",DBNull.Value),
                new MySqlParameter("p_state",DBNull.Value  ),
                new MySqlParameter("p_dist",DBNull.Value),
                new MySqlParameter("p_vill",DBNull.Value),
                msg
            });
            return Content(JsonConvert.SerializeObject(dt), "application/json");
            
        }

        public ActionResult GetOneUser(int? id)
        {
            if (id.HasValue)
            {
                MySqlParameter msg = new MySqlParameter("p_res", MySqlDbType.VarChar, 255);
                msg.Direction = ParameterDirection.Output;

                DataTable dt = db.table("sp_SignUp", new MySqlParameter[]{
                new MySqlParameter("p_action" , "SelectOne"),
                new MySqlParameter("p_id", id),
                new MySqlParameter("p_name",DBNull.Value),
                new MySqlParameter("p_email",DBNull.Value),
                new MySqlParameter("p_mob",DBNull.Value),
                new MySqlParameter("p_pass",DBNull.Value),
                new MySqlParameter("p_dob",DBNull.Value),
                new MySqlParameter("p_img", DBNull.Value),
                new MySqlParameter("p_gender",DBNull.Value),
                new MySqlParameter("p_hobby",DBNull.Value),
                new MySqlParameter("p_profession",DBNull.Value),
                new MySqlParameter("p_pincode",DBNull.Value),
                new MySqlParameter("p_state",DBNull.Value  ),
                new MySqlParameter("p_dist",DBNull.Value),
                new MySqlParameter("p_vill",DBNull.Value),
                msg
            });
                return Content(JsonConvert.SerializeObject(dt), "application/json");
            }
            else
            {
                return Json(new {success = false});
            }
            
        }
        public ActionResult Logout()
        {
            Response.Cookies.Delete("UEmail");
            Response.Cookies.Delete("UId");
            return RedirectToAction("Login");
        }
         
    }
}

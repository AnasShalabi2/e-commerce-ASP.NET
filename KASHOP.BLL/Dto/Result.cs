using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Dto
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public String Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public static Result<T> Ok(T? data, string message = "Success")
        {
            if(data is null)
            {
                return new Result<T> { Success = true, Message = message};

            }
            return new Result<T> { Success = true, Message = message, Data = data };
        }
        public static Result<T> Fail(string message)
        {
            return new Result<T> { Success = false, Message = message };
        }
        public static Result<T> Fail(string message,List<string> errors)
        {
            return new Result<T> { Success = false, Message = message , Errors= errors};
        }

    }
    }


using AutoMapper;
using BookStore.Data.Domain;
using BookStore.Models.Dto;

namespace BookStore.Models
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //CreateMap<Book, BookSummaryDto>();
            CreateMap<Book, BookDto>();
        }
    }
}

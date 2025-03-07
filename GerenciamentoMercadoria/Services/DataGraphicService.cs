﻿using DotSupply.Services.Interfaces;
using DotSupply.Models;
using Microsoft.EntityFrameworkCore;

namespace DotSupply.Services
{
    public class DataGraphicService : IDataGraphicService
    {
        private readonly DotSupplyContext _context;

        public DataGraphicService(DotSupplyContext context)
        {
            _context = context;
        }

        public async Task<List<ViewModelData>> Datas()
        {

            var tempEntrada = await _context.EntradaMercadoria.GroupBy(x => new { x.Data.Month, x.MercadoriaId }).Select(x =>
                new ViewModelData
                {
                    Nome = "ent " + x.First().Mercadoria.Nome,
                    Quantidade = x.Sum(x => x.Quantidade),
                    Months = x.First().Data.Month,
                    Id = x.First().Id
                }
            ).ToListAsync();

            var tempSaida = await _context.SaidaMercadoria.GroupBy(x => new { x.Data.Month, x.MercadoriaId }).Select(x =>
                new ViewModelData
                {
                    Nome = "sai " + x.First().Mercadoria.Nome,
                    Quantidade = x.Sum(x => x.Quantidade),
                    Months = x.First().Data.Month,
                    Id = x.First().Id
                }
            ).ToListAsync();

            var data = new List<ViewModelData>();

            data.AddRange(MapData(tempEntrada));

            data.AddRange(MapData(tempSaida));

            return data;
        }

        private List<ViewModelData> MapData(List<ViewModelData> temp)
        {
            var data = new List<ViewModelData>();

            data.AddRange(temp.GroupBy(x => x.Nome).Select(x =>
                new ViewModelData
                {
                    Nome = x.First().Nome,
                    Quantidade = x.First().Quantidade,
                    Months = x.First().Months,
                    Id = x.First().Id
                }
            ).ToList());

            foreach (var item in data)
            {
                item.ListData.Add(item.Quantidade);
                item.ListMonth.Add(item.Months);

                foreach (var itemtemp in temp)
                {
                    if (item.Nome == itemtemp.Nome && item.Id != itemtemp.Id)
                    {
                        item.ListData.Add(itemtemp.Quantidade);
                        item.ListMonth.Add(itemtemp.Months);
                    }
                }
            }

            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    if (!data[i].ListMonth.Contains(j + 1))
                        data[i].ListData.Insert(j, -1);
                }
            }

            string[] color = new string[] { "red", "blue", "yellow", "green", "purple", "geige", "orange", "pink", "navy blue", "brown", "burgundy", "khaki" };
            for (int i = 0; i < data.Count; i++)
            {
                data[i].borderColor = color[i];
            }

            return data;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NorthwindWebApiApp.Models;

namespace NorthwindWebApiApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly NorthwindModel.NorthwindEntities _entities;

        public OrderService()
        {
            _entities = new NorthwindModel.NorthwindEntities(new Uri("https://services.odata.org/V3/Northwind/Northwind.svc"));
        }

        public async Task<IEnumerable<BriefOrderDescription>> GetOrdersAsync()
        {
            var orderTaskFactory = new TaskFactory<IEnumerable<NorthwindModel.Order>>();
            var orders = (await orderTaskFactory.FromAsync(
                _entities.Orders.BeginExecute(null, null),
                iar => _entities.Orders.EndExecute(iar)));

            return orders.Select(o => new BriefOrderDescription
            {
                OrderId = o.OrderID,
                OrderDate = o.OrderDate,
                RequiredDate = o.RequiredDate
            });
        }

        public async Task<FullOrderDescription> GetOrderAsync(int orderId)
        {
            var orderQueryTaskFactory = new TaskFactory<IEnumerable<NorthwindModel.Orders_Qry>>();
            var query = _entities.Orders_Qries.AddQueryOption("$filter", $"OrderID eq {orderId}");

            var orders = (await orderQueryTaskFactory.FromAsync(
                query.BeginExecute(null, null),
                iar => query.EndExecute(iar))).ToArray();

            var order = orders.FirstOrDefault();

            if (order == null)
            {
                return null;
            }

            return new FullOrderDescription
            {
                OrderId = order.OrderID,
                CustomerId = order.CustomerID,
                EmployeeId = order.EmployeeID,
                OrderDate = order.OrderDate,
                RequiredDate = order.RequiredDate,
                ShipVia = order.ShipVia
            };
        }
    }
}
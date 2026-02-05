using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels.Car;
using Microsoft.Extensions.Options;
using Nest;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace Repositories.Repositories
{
    public class VehicleInspectionRepository : IVehicleInspectionRepository
    {
        private readonly VehicleInspectionDAL _VehicleInspectionDAL;
        private readonly AllCodeDAL _AllCodeDAL;
        public VehicleInspectionRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            _VehicleInspectionDAL = new VehicleInspectionDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _AllCodeDAL = new AllCodeDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        //danh sách xe đăng ký
        public async Task<List<CartoFactoryModel>> GetListRegisteredVehicle(CartoFactorySearchModel searchModel)
        {
            try
            {
                var TIME_RESET = await _AllCodeDAL.GetListSortByName(AllCodeType.TIME_RESET);
                var hours = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                            ? TIME_RESET[0].UpdateTime.Value.Hour
                            : 17;
                var minutes = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                              ? TIME_RESET[0].UpdateTime.Value.Minute
                              : 55;
                var now = DateTime.Now;
                var expireAt = new DateTime(now.Year, now.Month, now.Day, hours, minutes, 0);

                searchModel.RegistrationTime = expireAt;

                return await _VehicleInspectionDAL.GetListRegisteredVehicle(searchModel);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListRegisteredVehicle - VehicleInspectionRepository: " + ex);
            }
            return null;
        }
        public async Task<List<CartoFactoryModel>> GetListCartoFactory(CartoFactorySearchModel searchModel)
        {
            try
            {
                var TIME_RESET = await _AllCodeDAL.GetListSortByName(AllCodeType.TIME_RESET);
                var hours = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                            ? TIME_RESET[0].UpdateTime.Value.Hour
                            : 17;
                var minutes = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                              ? TIME_RESET[0].UpdateTime.Value.Minute
                              : 55;
                var now = DateTime.Now;
                var expireAt = new DateTime(now.Year, now.Month, now.Day, hours, minutes, 0);

                searchModel.RegistrationTime = expireAt;

                return await _VehicleInspectionDAL.GetListCartoFactory(searchModel);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListCartoFactory - VehicleInspectionRepository: " + ex);
            }
            return null;
        }
        public async Task<int> UpdateCar(VehicleInspectionUpdateModel model)
        {
            try
            {
                return await _VehicleInspectionDAL.UpdateVehicleInspection(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateVehicleInspection - VehicleInspectionRepository: " + ex);
                return -1;
            }
        }

        public async Task<CartoFactoryModel> GetDetailtVehicleInspection(int id)
        {
            try
            {
                return await _VehicleInspectionDAL.GetDetailtVehicleInspection(id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListCartoFactory - VehicleInspectionRepository: " + ex);
            }
            return null;
        }
        public int SaveVehicleInspection(RegistrationRecord model)
        {
            try
            {
                return _VehicleInspectionDAL.SaveVehicleInspection(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SaveVehicleInspection - VehicleInspectionRepository: " + ex);
            }
            return 0;
        }
        public Task<string> GetAudioPathByVehicleNumber(string VehicleNumber)
        {
            try
            {
                return _VehicleInspectionDAL.GetAudioPathByVehicleNumber(VehicleNumber);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SaveVehicleInspection - VehicleInspectionRepository: " + ex);
            }
            return null;
        }
        public async Task<List<CartoFactoryModel>> GetListVehicleInspectionSynthetic(DateTime? FromDate, DateTime? ToDate, int LoadType)
        {
            try
            {
                var TIME_RESET = await _AllCodeDAL.GetListSortByName(AllCodeType.TIME_RESET);
                var hours = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                            ? TIME_RESET[0].UpdateTime.Value.Hour
                            : 17;
                var minutes = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                              ? TIME_RESET[0].UpdateTime.Value.Minute
                              : 55;
                var now = DateTime.Now;
                var expireAt = new DateTime(now.Year, now.Month, now.Day, hours, minutes, 0);
                if (ToDate != null)
                {
                    ToDate = ((DateTime)ToDate).Date.AddHours(hours).AddMinutes(minutes).AddSeconds(0);
                }
                else
                {
                    ToDate = expireAt;
                }
               
           
                return await _VehicleInspectionDAL.GetListVehicleInspectionSynthetic(FromDate, ToDate, LoadType);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListVehicleInspectionSynthetic - VehicleInspectionRepository: " + ex);
            }
            return null;
        }
        public async Task<Entities.ViewModels.TotalVehicleInspection> CountTotalVehicleInspectionSynthetic(DateTime? FromDate, DateTime? ToDate)
        {
            try
            {
                var TIME_RESET = await _AllCodeDAL.GetListSortByName(AllCodeType.TIME_RESET);
                var hours = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                            ? TIME_RESET[0].UpdateTime.Value.Hour
                            : 17;
                var minutes = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                              ? TIME_RESET[0].UpdateTime.Value.Minute
                              : 55;
                var now = DateTime.Now;
                var expireAt = new DateTime(now.Year, now.Month, now.Day, hours, minutes, 0);
                if (ToDate != null)
                {
                    ToDate = ((DateTime)ToDate).Date.AddHours(hours).AddMinutes(minutes).AddSeconds(0);
                }
                else
                {
                    ToDate = expireAt;
                }
                return await _VehicleInspectionDAL.CountTotalVehicleInspectionSynthetic(FromDate, ToDate);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListVehicleInspectionSynthetic - VehicleInspectionRepository: " + ex);
            }
            return null;
        }
        public async Task<List<TotalWeightByHourModel>> GetTotalWeightByHour(DateTime? RegistrationTime)
        {
            try
            {
                var TIME_RESET = await _AllCodeDAL.GetListSortByName(AllCodeType.TIME_RESET);
                var hours = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                            ? TIME_RESET[0].UpdateTime.Value.Hour
                            : 17;
                var minutes = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                              ? TIME_RESET[0].UpdateTime.Value.Minute
                              : 55;
                var now = DateTime.Now;
                var expireAt = new DateTime(now.Year, now.Month, now.Day, hours, minutes, 0);
                RegistrationTime = expireAt;
                return await _VehicleInspectionDAL.GetTotalWeightByHour(RegistrationTime);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTotalWeightByHour - VehicleInspectionRepository: " + ex);
            }
            return null;
        }
        public async Task<List<TotalWeightByHourModel>> GetTotalWeightByWeightGroup(DateTime? RegistrationTime)
        {
            try
            {

                return await _VehicleInspectionDAL.GetTotalWeightByWeightGroup(RegistrationTime);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTotalWeightByHour - VehicleInspectionRepository: " + ex);
            }
            return null;
        }
        public async Task<List<TotalWeightByHourModel>> GetTotalWeightByTroughType(DateTime? RegistrationTime)
        {
            try
            {

                return await _VehicleInspectionDAL.GetTotalWeightByTroughType(RegistrationTime);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTotalWeightByTroughType - VehicleInspectionRepository: " + ex);
            }
            return null;
        }

        public async Task<List<CartoFactoryModel>> GetListVehicleProcessingIsLoading(CartoFactorySearchModel searchModel)
        {
            try
            {
                var TIME_RESET = await _AllCodeDAL.GetListSortByName(AllCodeType.TIME_RESET);
                var hours = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                            ? TIME_RESET[0].UpdateTime.Value.Hour
                            : 17;
                var minutes = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                              ? TIME_RESET[0].UpdateTime.Value.Minute
                              : 55;
                var now = DateTime.Now;
                var expireAt = new DateTime(now.Year, now.Month, now.Day, hours, minutes, 0);
                searchModel.RegistrationTime = expireAt;
                return await _VehicleInspectionDAL.GetListVehicleProcessingIsLoading(searchModel);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListVehicleProcessingIsLoading - VehicleInspectionRepository: " + ex);
            }
            return null;
        }
        public async Task<List<CartoFactoryModel>> GetListVehicleWeighedInput(CartoFactorySearchModel searchModel)
        {
            try
            {
                var TIME_RESET = await _AllCodeDAL.GetListSortByName(AllCodeType.TIME_RESET);
                var hours = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                            ? TIME_RESET[0].UpdateTime.Value.Hour
                            : 17;
                var minutes = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                              ? TIME_RESET[0].UpdateTime.Value.Minute
                              : 55;
                var now = DateTime.Now;
                var expireAt = new DateTime(now.Year, now.Month, now.Day, hours, minutes, 0);
                searchModel.RegistrationTime = expireAt;
                return await _VehicleInspectionDAL.GetListVehicleWeighedInput(searchModel);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListVehicleWeighedInput - VehicleInspectionRepository: " + ex);
            }
            return null;
        }
        public async Task<List<CartoFactoryModel>> GetListVehicleCarCallList(CartoFactorySearchModel searchModel)
        {
            try
            {
                var TIME_RESET = await _AllCodeDAL.GetListSortByName(AllCodeType.TIME_RESET);
                var hours = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                            ? TIME_RESET[0].UpdateTime.Value.Hour
                            : 17;
                var minutes = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                              ? TIME_RESET[0].UpdateTime.Value.Minute
                              : 55;
                var now = DateTime.Now;
                var expireAt = new DateTime(now.Year, now.Month, now.Day, hours, minutes, 0);
                searchModel.RegistrationTime = expireAt;
                return await _VehicleInspectionDAL.GetListVehicleCarCallList(searchModel);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListVehicleCarCallList - VehicleInspectionRepository: " + ex);
            }
            return null;
        }
        public async Task<List<CartoFactoryModel>> GetListVehicleCallTheScale(CartoFactorySearchModel searchModel)
        {
            try
            {
                var TIME_RESET = await _AllCodeDAL.GetListSortByName(AllCodeType.TIME_RESET);
                var hours = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                            ? TIME_RESET[0].UpdateTime.Value.Hour
                            : 17;
                var minutes = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                              ? TIME_RESET[0].UpdateTime.Value.Minute
                              : 55;
                var now = DateTime.Now;
                var expireAt = new DateTime(now.Year, now.Month, now.Day, hours, minutes, 0);
                searchModel.RegistrationTime = expireAt;
                return await _VehicleInspectionDAL.GetListVehicleCallTheScale(searchModel);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListVehicleCallTheScale - VehicleInspectionRepository: " + ex);
            }
            return null;
        }
        public async Task<List<CartoFactoryModel>> GetListVehicleListVehicles(CartoFactorySearchModel searchModel)
        {
            try
            {
                var TIME_RESET = await _AllCodeDAL.GetListSortByName(AllCodeType.TIME_RESET);
                var hours = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                            ? TIME_RESET[0].UpdateTime.Value.Hour
                            : 17;
                var minutes = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                              ? TIME_RESET[0].UpdateTime.Value.Minute
                              : 55;
                var now = DateTime.Now;
                var expireAt = new DateTime(now.Year, now.Month, now.Day, hours, minutes, 0);
                searchModel.RegistrationTime = expireAt;
                return await _VehicleInspectionDAL.GetListVehicleListVehicles(searchModel);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListVehicleListVehicles - VehicleInspectionRepository: " + ex);
            }
            return null;
        }

     
        public async Task<int> UpdateVehicleInspectionByVehicleNumber(string VehicleNumber)
        {
            try
            {
                return await _VehicleInspectionDAL.UpdateVehicleInspectionByVehicleNumber(VehicleNumber);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SaveVehicleInspection - VehicleInspectionRepository: " + ex);
            }
            return 0;
        } 
        public async Task<int> UpdateVehicleLoadTaken(int Id, int VehicleLoadTaken)
        {
            try
            {
                return await _VehicleInspectionDAL.UpdateVehicleLoadTaken(Id, VehicleLoadTaken);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateVehicleLoadTaken - VehicleInspectionRepository: " + ex);
            }
            return 0;
        }
    }
}

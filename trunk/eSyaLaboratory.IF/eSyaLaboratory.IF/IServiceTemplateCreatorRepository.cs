﻿using eSyaLaboratory.DO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace eSyaLaboratory.IF
{
    public interface IServiceTemplateCreatorRepository
    {
        Task<List<DO_ResultType>> GetResultTypes();
        Task<List<DO_ServiceTemplateCreator>> GetServiceTemplateByBKeyServiceClass(int businessKey, int serviceClass);
        Task<DO_ServiceTemplateCreator> GetServiceTemplateByBKeyServiceID(int businessKey, int serviceID);
        Task<DO_ReturnParameter> AddOrUpdateServiceTemplateMaster(DO_ServiceTemplateCreator obj);
        #region ShortResult
        Task<DO_ServiceTemplateFull> GetServiceShortValuesByBKeyServiceID(int businessKey, int serviceID);
        Task<DO_ReturnParameter> AddOrUpdateServiceShortValues(DO_ServiceTemplateFull obj);
        #endregion
        #region LongResult
        Task<DO_ServiceTemplateFull> GetServiceLongValuesByBKeyServiceID(int businessKey, int serviceID);
        Task<DO_ReturnParameter> AddOrUpdateServiceLongValues(DO_ServiceTemplateFull obj);
        #endregion
        #region Analysis Profile
        Task<DO_ServiceTemplateFull> GetServiceAnalysisValuesByBKeyServiceID(int businessKey, int serviceID);
        Task<DO_ReturnParameter> AddOrUpdateServiceAnalysisValues(DO_ServiceTemplateFull obj);
        #endregion
        #region Descriptive Result
        Task<DO_ServiceTemplateFull> GetServiceDescriptiveResultByBKeyServiceID(int businessKey, int serviceID, string templateType);
        Task<DO_ReturnParameter> AddOrUpdateServiceDescriptiveResult(DO_ServiceTemplateFull obj);
        #endregion
        #region CommonValues
        Task<DO_ReturnParameter> AddOrUpdateServiceCommonValues(DO_ServiceCommonValues obj);
        Task<DO_ServiceCommonValues> GetServiceCommonValuesByBKeyServiceID(int businessKey, int serviceID);
        #endregion
    }
}

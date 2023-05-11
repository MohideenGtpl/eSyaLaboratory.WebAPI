﻿using eSyaLaboratory.DL.Entities;
using eSyaLaboratory.DO;
using eSyaLaboratory.IF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSyaLaboratory.DL.Repository
{
    public class ServiceTemplateCreatorRepository : IServiceTemplateCreatorRepository
    {

        public async Task<List<DO_ResultType>> GetResultTypes()
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtElarty
                        .Where(w => w.ActiveStatus)
                                 .Select(x => new DO_ResultType
                                 {
                                     ResultType = x.ResultType,
                                     ResultTypeDesc = x.ResultTypeDesc,
                                 }
                        ).ToListAsync();
                    return await result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<DO_ServiceTemplateCreator>> GetServiceTemplateByBKeyServiceClass(int businessKey, int serviceClass)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtEssrbl.Where(w => w.BusinessKey == businessKey)
                        .Join(db.GtEssrms.Where(w => w.ServiceClassId == serviceClass),
                        b => b.ServiceId,
                        s => s.ServiceId,
                        (b, s) => new { b, s })
                        .GroupJoin(db.GtElasms.Where(w => w.BusinessKey == businessKey),
                            bs => bs.s.ServiceId,
                            t => t.ServiceId,
                            (bs, t) => new { bs, t = t.FirstOrDefault() })
                        .GroupJoin(db.GtElarty,
                            bst => new { ResultType = bst.t != null ? bst.t.ResultType : "" },
                            r => new { r.ResultType },
                            (bst, r) => new { bst, r = r.FirstOrDefault() })
                        .GroupJoin(db.GtEcapcd,
                            bstr => new { SampleType = bstr.bst.t != null ? bstr.bst.t.SampleType : 0 },
                            m => new { SampleType = m.ApplicationCode },
                            (bstr, m) => new { bstr, m = m.FirstOrDefault() })
                        .Select(x => new DO_ServiceTemplateCreator
                        {
                            ServiceId = x.bstr.bst.bs.s.ServiceId,
                            ServiceShortDesc = x.bstr.bst.bs.s.ServiceShortDesc,
                            ServiceDesc = x.bstr.bst.bs.s.ServiceDesc,
                            ResultType = x.bstr.bst.t != null ? x.bstr.bst.t.ResultType : " ",
                            ResultTypeDesc = x.bstr.r != null ? x.bstr.r.ResultTypeDesc : " ",
                            SampleType = x.bstr.bst.t != null ? x.bstr.bst.t.SampleType : 0,
                            SampleTypeDesc = x.m != null ? x.m.CodeDesc : " ",
                            LabPrintSequence = x.bstr.bst.t != null ? x.bstr.bst.t.LabPrintSequence : 0,
                            TimeRequiredForReport = x.bstr.bst.t != null ? x.bstr.bst.t.TimeRequiredForReport : 0,


                            IsResultCreated = x.bstr.bst.t != null ? true : false,

                        }
                        ).ToListAsync();
                    return await result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DO_ServiceTemplateCreator> GetServiceTemplateByBKeyServiceID(int businessKey, int serviceID)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtElasms.Where(w => w.BusinessKey == businessKey && w.ServiceId == serviceID)
                        .Join(db.GtEssrms,
                        t => t.ServiceId,
                        s => s.ServiceId,
                        (t, s) => new { t, s })
                                 .Select(x => new DO_ServiceTemplateCreator
                                 {
                                     ServiceId = x.t.ServiceId,
                                     ServiceDesc = x.s.ServiceDesc,
                                     ResultType = x.t.ResultType,
                                     SampleType = x.t.SampleType,
                                     LabPrintSequence = x.t.LabPrintSequence,
                                     TimeRequiredForReport = x.t.TimeRequiredForReport,

                                 }
                        ).FirstOrDefaultAsync();
                    return await result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DO_ReturnParameter> AddOrUpdateServiceTemplateMaster(DO_ServiceTemplateCreator obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var RecordExist = db.GtElasms.Where(w => w.BusinessKey == obj.BusinessKey && w.ServiceId == obj.ServiceId).FirstOrDefault();
                        if (RecordExist != null)
                        {
                            RecordExist.ResultType = obj.ResultType;
                            RecordExist.SampleType = obj.SampleType;
                            RecordExist.LabPrintSequence = obj.LabPrintSequence;
                            RecordExist.TimeRequiredForReport = obj.TimeRequiredForReport;
                            RecordExist.ActiveStatus = obj.ActiveStatus;
                            RecordExist.ModifiedBy = obj.UserID;
                            RecordExist.ModifiedOn = obj.CreatedOn;
                            RecordExist.ModifiedTerminal = obj.TerminalID;
                        }
                        else
                        {
                            var servicetemplatemaster = new GtElasms
                            {
                                BusinessKey = obj.BusinessKey,
                                ServiceId = obj.ServiceId,
                                ResultType = obj.ResultType,
                                SampleType = obj.SampleType,
                                LabPrintSequence = obj.LabPrintSequence,
                                TimeRequiredForReport = obj.TimeRequiredForReport,
                                ActiveStatus = obj.ActiveStatus,
                                FormId = obj.FormId,
                                CreatedBy = obj.UserID,
                                CreatedOn = obj.CreatedOn,
                                CreatedTerminal = obj.TerminalID
                            };
                            db.GtElasms.Add(servicetemplatemaster);

                        }

                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        return new DO_ReturnParameter { Status = true };


                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }
        }
        #region ShortResult
        public async Task<DO_ServiceTemplateFull> GetServiceShortValuesByBKeyServiceID(int businessKey, int serviceID)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var obj = new DO_ServiceTemplateFull();
                    //Get Common values
                    obj.CommonValues = await GetServiceCommonValuesByBKeyServiceID(businessKey, serviceID);
                    //Get Short Header
                    var shortheader = db.GtElassu.Where(w => w.BusinessKey == businessKey && w.ServiceId == serviceID)
                        .Select(x => new DO_ShortValueHeader
                                {
                                    ServiceId = x.ServiceId,
                                    Unit = x.Unit,
                                    IsNumericResultValue = x.IsNumericResultValue,
                                    ResultComputed = x.ResultComputed,
                                    ResultFormula = x.ResultFormula,
                                }
                                ).FirstOrDefaultAsync();
                    obj.ShortHeader = await shortheader;
                    //Get Short Values
                    var shortvalues = db.GtElassr.Where(w => w.BusinessKey == businessKey && w.ServiceId == serviceID)
                        .Select(x => new DO_ShortNormalValue
                        {
                            ServiceId = x.ServiceId,
                            SerialNumber = x.SerialNumber,
                            TestParameter = x.TestParameter,
                            Sex = x.Sex,
                            StartAge = x.StartAge,
                            StartAgeType = x.StartAgeType,
                            EndAge = x.EndAge,
                            EndAgeType = x.EndAgeType,
                            MinValue = x.MinValue,
                            MaxValue = x.MaxValue,
                            NormalValues = x.NormalValues,
                            HypoValue = x.HypoValue,
                            HyperValue = x.HyperValue,
                            Desirable = x.Desirable,
                            BorderLine = x.BorderLine,
                            HighRisk = x.HighRisk

                        }
                        ).ToListAsync();
                    obj.l_ShortValues = await shortvalues;
                    //Get Test Method
                    var testmethod = db.GtElastm.Where(w => w.BusinessKey == businessKey && w.ServiceId == serviceID)
                        .Select(x => new DO_TestMethod
                        {
                            ServiceId = x.ServiceId,
                            TestMethod = x.TestMethod,
                            ActiveStatus = x.ActiveStatus,   
                        }
                        ).ToListAsync();
                    obj.l_TestMethod = await testmethod;


                    return obj;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DO_ReturnParameter> AddOrUpdateServiceShortValues(DO_ServiceTemplateFull obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Update common values
                        var response = await AddOrUpdateServiceCommonValues(obj.CommonValues);
                        if (!response.Status)
                        {
                            return new DO_ReturnParameter() { Status = false, Message = response.Message };
                        }
                        //Update Short Header
                        var existheader = db.GtElassu.Where(w => w.BusinessKey == obj.CommonValues.BusinessKey && w.ServiceId == obj.CommonValues.ServiceId).FirstOrDefault();
                        if (existheader != null)
                        {
                            if (existheader.Unit != obj.ShortHeader.Unit || existheader.IsNumericResultValue != obj.ShortHeader.IsNumericResultValue || existheader.ResultComputed != obj.ShortHeader.ResultComputed || existheader.ResultFormula != obj.ShortHeader.ResultFormula)
                            {
                                existheader.Unit = obj.ShortHeader.Unit;
                                existheader.IsNumericResultValue = obj.ShortHeader.IsNumericResultValue;
                                existheader.ResultComputed = obj.ShortHeader.ResultComputed;
                                existheader.ResultFormula = obj.ShortHeader.ResultFormula;

                                existheader.ModifiedBy = obj.CommonValues.UserID;
                                existheader.ModifiedOn = obj.CommonValues.CreatedOn;
                                existheader.ModifiedTerminal = obj.CommonValues.TerminalID;
                            }

                        }
                        else
                        {
                            var shortheader = new GtElassu
                            {
                                BusinessKey = obj.CommonValues.BusinessKey,
                                ServiceId = obj.CommonValues.ServiceId,
                                Unit = obj.ShortHeader.Unit,
                                IsNumericResultValue = obj.ShortHeader.IsNumericResultValue,
                                ResultComputed = obj.ShortHeader.ResultComputed,
                                ResultFormula = obj.ShortHeader.ResultFormula,

                                ActiveStatus = true,
                                FormId = obj.CommonValues.FormId,
                                CreatedBy = obj.CommonValues.UserID,
                                CreatedOn = obj.CommonValues.CreatedOn,
                                CreatedTerminal = obj.CommonValues.TerminalID
                            };
                            db.GtElassu.Add(shortheader);
                        }
                        // Update Short values
                        var newSerial = db.GtElassr.Where(w => w.BusinessKey == obj.CommonValues.BusinessKey && w.ServiceId == obj.CommonValues.ServiceId).Select(a => (int)a.SerialNumber).DefaultIfEmpty(0).Max() + 1;
                        foreach (var short_value in obj.l_ShortValues)
                        {
                            if (short_value.SerialNumber == 0)
                            {
                                var serviceshortvalue = new GtElassr
                                {
                                    BusinessKey = obj.CommonValues.BusinessKey,
                                    ServiceId = obj.CommonValues.ServiceId,
                                    SerialNumber = newSerial,
                                    TestParameter = "0",
                                    Sex = short_value.Sex,
                                    StartAge = short_value.StartAge,
                                    StartAgeType = short_value.StartAgeType,
                                    EndAge = short_value.EndAge,
                                    EndAgeType = short_value.EndAgeType,
                                    MinValue = short_value.MinValue,
                                    MaxValue = short_value.MaxValue,
                                    NormalValues = short_value.NormalValues,
                                    HypoValue = short_value.HypoValue,
                                    HyperValue = short_value.HyperValue,

                                    Desirable = 0,
                                    BorderLine = 0,
                                    HighRisk = 0,

                                    ActiveStatus = true,
                                    FormId = obj.CommonValues.FormId,
                                    CreatedBy = obj.CommonValues.UserID,
                                    CreatedOn = obj.CommonValues.CreatedOn,
                                    CreatedTerminal = obj.CommonValues.TerminalID
                                };
                                db.GtElassr.Add(serviceshortvalue);
                                newSerial += 1;
                            }
                            else
                            {

                                var ExistRecord = db.GtElassr.Where(w => w.BusinessKey == obj.CommonValues.BusinessKey && w.ServiceId == obj.CommonValues.ServiceId && w.SerialNumber == short_value.SerialNumber).FirstOrDefault();
                                int flag = 0;
                                if (ExistRecord.Sex != short_value.Sex || ExistRecord.StartAge != short_value.StartAge || ExistRecord.StartAgeType != short_value.StartAgeType || ExistRecord.EndAge != short_value.EndAge || ExistRecord.EndAgeType != short_value.EndAgeType)
                                {
                                    ExistRecord.Sex = short_value.Sex;
                                    ExistRecord.StartAge = short_value.StartAge;
                                    ExistRecord.StartAgeType = short_value.StartAgeType;
                                    ExistRecord.EndAge = short_value.EndAge;
                                    ExistRecord.EndAgeType = short_value.EndAgeType;
                                    flag = 1;
                                }
                                if (ExistRecord.MinValue != short_value.MinValue || ExistRecord.MaxValue != short_value.MaxValue || ExistRecord.NormalValues != short_value.NormalValues)
                                {
                                    ExistRecord.MinValue = short_value.MinValue;
                                    ExistRecord.MaxValue = short_value.MaxValue;
                                    ExistRecord.NormalValues = short_value.NormalValues;
                                    flag = 1;
                                }
                                if (ExistRecord.HypoValue != short_value.HypoValue || ExistRecord.HyperValue != short_value.HyperValue)
                                {
                                    ExistRecord.HypoValue = short_value.HypoValue;
                                    ExistRecord.HyperValue = short_value.HyperValue;
                                    flag = 1;
                                }

                                if (flag == 1)
                                {
                                    ExistRecord.ModifiedBy = obj.CommonValues.UserID;
                                    ExistRecord.ModifiedOn = obj.CommonValues.CreatedOn;
                                    ExistRecord.ModifiedTerminal = obj.CommonValues.TerminalID;
                                }
                            }
                        }
                        // Update Test Method
                        foreach (var test_method in obj.l_TestMethod)
                        {
                            var ExistRecord = db.GtElastm.Where(w => w.BusinessKey == obj.CommonValues.BusinessKey && w.ServiceId == obj.CommonValues.ServiceId).FirstOrDefault();
                            if (ExistRecord != null)
                            {
                                if (ExistRecord.ActiveStatus != test_method.ActiveStatus)
                                {
                                    ExistRecord.ActiveStatus = test_method.ActiveStatus;
                                    ExistRecord.ModifiedBy = obj.CommonValues.UserID;
                                    ExistRecord.ModifiedOn = obj.CommonValues.CreatedOn;
                                    ExistRecord.ModifiedTerminal = obj.CommonValues.TerminalID;
                                }
                            }
                            else
                            {
                                var testmethod = new GtElastm
                                {
                                    BusinessKey = obj.CommonValues.BusinessKey,
                                    ServiceId = obj.CommonValues.ServiceId,
                                    TestMethod = test_method.TestMethod,
                                    ActiveStatus= test_method.ActiveStatus,


                                    FormId = obj.CommonValues.FormId,
                                    CreatedBy = obj.CommonValues.UserID,
                                    CreatedOn = obj.CommonValues.CreatedOn,
                                    CreatedTerminal = obj.CommonValues.TerminalID
                                };
                                db.GtElastm.Add(testmethod);

                            }    
                               
                            
                        }
                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        return new DO_ReturnParameter { Status = true };


                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        return new DO_ReturnParameter() { Status = false, Message = ex.Message };
                    }
                }
            }
        }
        #endregion
        #region LongResult
        public async Task<DO_ServiceTemplateFull> GetServiceLongValuesByBKeyServiceID(int businessKey, int serviceID)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var obj = new DO_ServiceTemplateFull();
                    //Get Common values
                    obj.CommonValues = await GetServiceCommonValuesByBKeyServiceID(businessKey, serviceID);
                    
                    //Get Long Values
                    var longvalues = db.GtElaslp.Where(w => w.BusinessKey == businessKey && w.ServiceId == serviceID)
                        .Select(x => new DO_LongValue
                        {
                            ServiceId = x.ServiceId,
                            ProfileServiceId = x.ProfileServiceId,
                            ReportingSequence = x.ReportingSequence,
                            ActiveStatus = x.ActiveStatus,                           
                        }
                        ).ToListAsync();
                    obj.l_LongValues = await longvalues;
                    


                    return obj;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DO_ReturnParameter> AddOrUpdateServiceLongValues(DO_ServiceTemplateFull obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Update common values
                        var response = await AddOrUpdateServiceCommonValues(obj.CommonValues);
                        if (!response.Status)
                        {
                            return new DO_ReturnParameter() { Status = false, Message = response.Message };
                        }
                        // Update Long values
                        foreach (var long_value in obj.l_LongValues)
                        {


                            var ExistRecord = db.GtElaslp.Where(w => w.BusinessKey == obj.CommonValues.BusinessKey && w.ServiceId == obj.CommonValues.ServiceId && w.ProfileServiceId == long_value.ProfileServiceId).FirstOrDefault();
                            if (ExistRecord != null)
                            {
                                if (ExistRecord.ReportingSequence != long_value.ReportingSequence || ExistRecord.ActiveStatus != long_value.ActiveStatus)
                                {
                                    ExistRecord.ReportingSequence = long_value.ReportingSequence;
                                    ExistRecord.ActiveStatus = long_value.ActiveStatus;

                                    ExistRecord.ModifiedBy = obj.CommonValues.UserID;
                                    ExistRecord.ModifiedOn = obj.CommonValues.CreatedOn;
                                    ExistRecord.ModifiedTerminal = obj.CommonValues.TerminalID;
                                }

                            }
                            else
                            {
                                var servicelongvalue = new GtElaslp
                                {
                                    BusinessKey = obj.CommonValues.BusinessKey,
                                    ServiceId = obj.CommonValues.ServiceId,
                                    ProfileServiceId = long_value.ProfileServiceId,
                                    ReportingSequence = long_value.ReportingSequence,
                                    ActiveStatus = long_value.ActiveStatus,

                                    FormId = obj.CommonValues.FormId,
                                    CreatedBy = obj.CommonValues.UserID,
                                    CreatedOn = obj.CommonValues.CreatedOn,
                                    CreatedTerminal = obj.CommonValues.TerminalID
                                };
                                db.GtElaslp.Add(servicelongvalue);

                            }


                        }
                        
                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        return new DO_ReturnParameter { Status = true };


                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        return new DO_ReturnParameter() { Status = false, Message = ex.Message };
                    }
                }
            }
        }
        #endregion
        #region Analysis Profile
        public async Task<DO_ServiceTemplateFull> GetServiceAnalysisValuesByBKeyServiceID(int businessKey, int serviceID)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var obj = new DO_ServiceTemplateFull();
                    //Get Common values
                    obj.CommonValues = await GetServiceCommonValuesByBKeyServiceID(businessKey, serviceID);
                   
                    //Get Analysis Values
                    var analysisvalues = db.GtElasan.Where(w => w.BusinessKey == businessKey && w.ServiceId == serviceID)
                        .Select(x => new DO_AnalysisValue
                        {
                            ServiceId = x.ServiceId,
                            SerialNumber = x.SerialNumber,
                            Heading=x.Heading,
                            TestParameterDesc = x.TestParameterDesc,
                            Unit = x.Unit,
                            NormalValues = x.NormalValues,
                            TestMethod = x.TestMethod,
                            ReportingSequence = x.ReportingSequence,
                        }
                        ).ToListAsync();
                    obj.l_AnalysisValues = await analysisvalues;                   
                    return obj;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DO_ReturnParameter> AddOrUpdateServiceAnalysisValues(DO_ServiceTemplateFull obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Update common values
                        var response = await AddOrUpdateServiceCommonValues(obj.CommonValues);
                        if (!response.Status)
                        {
                            return new DO_ReturnParameter() { Status = false, Message = response.Message };
                        }
                        // Update Analysis values
                        var newSerial = db.GtElasan.Where(w => w.BusinessKey == obj.CommonValues.BusinessKey && w.ServiceId == obj.CommonValues.ServiceId).Select(a => (int)a.SerialNumber).DefaultIfEmpty(0).Max() + 1;
                        foreach (var analysis_value in obj.l_AnalysisValues)
                        {
                            if (analysis_value.SerialNumber == 0)
                            {
                                var serviceanalysisvalue = new GtElasan
                                {
                                    BusinessKey = obj.CommonValues.BusinessKey,
                                    ServiceId = obj.CommonValues.ServiceId,
                                    SerialNumber = newSerial,
                                    Heading= analysis_value.Heading,
                                    TestParameterDesc = "0",
                                    Unit=analysis_value.Unit,
                                    NormalValues=analysis_value.NormalValues,
                                    TestMethod=analysis_value.TestMethod,
                                    ReportingSequence=analysis_value.ReportingSequence,

                                    ActiveStatus = true,
                                    FormId = obj.CommonValues.FormId,
                                    CreatedBy = obj.CommonValues.UserID,
                                    CreatedOn = obj.CommonValues.CreatedOn,
                                    CreatedTerminal = obj.CommonValues.TerminalID
                                };
                                db.GtElasan.Add(serviceanalysisvalue);
                                newSerial += 1;
                            }
                            else
                            {

                                var ExistRecord = db.GtElasan.Where(w => w.BusinessKey == obj.CommonValues.BusinessKey && w.ServiceId == obj.CommonValues.ServiceId && w.SerialNumber == analysis_value.SerialNumber).FirstOrDefault();
                                int flag = 0;
                                if (ExistRecord.Heading != analysis_value.Heading || ExistRecord.TestParameterDesc != analysis_value.TestParameterDesc || ExistRecord.Unit != analysis_value.Unit)
                                {
                                    ExistRecord.Heading = analysis_value.Heading;
                                    ExistRecord.TestParameterDesc = analysis_value.TestParameterDesc;
                                    ExistRecord.Unit = analysis_value.Unit;
                                    flag = 1;
                                }
                                if (ExistRecord.NormalValues != analysis_value.NormalValues || ExistRecord.TestMethod != analysis_value.TestMethod || ExistRecord.ReportingSequence != analysis_value.ReportingSequence)
                                {
                                    ExistRecord.NormalValues = analysis_value.NormalValues;
                                    ExistRecord.TestMethod = analysis_value.TestMethod;
                                    ExistRecord.Unit = analysis_value.Unit;
                                    flag = 1;
                                }
                               
                                if (flag == 1)
                                {
                                    ExistRecord.ModifiedBy = obj.CommonValues.UserID;
                                    ExistRecord.ModifiedOn = obj.CommonValues.CreatedOn;
                                    ExistRecord.ModifiedTerminal = obj.CommonValues.TerminalID;
                                }
                            }
                        }
                       
                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        return new DO_ReturnParameter { Status = true };


                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        return new DO_ReturnParameter() { Status = false, Message = ex.Message };
                    }
                }
            }
        }
        #endregion
        #region Descriptive Result
        public async Task<DO_ServiceTemplateFull> GetServiceDescriptiveResultByBKeyServiceID(int businessKey, int serviceID, string templateType)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var obj = new DO_ServiceTemplateFull();
                    //Get Common values
                    obj.CommonValues = await GetServiceCommonValuesByBKeyServiceID(businessKey, serviceID);
                    //Get Descriptive Result
                    var descriptive = db.GtElasds.Where(w => w.BusinessKey == businessKey && w.ServiceId == serviceID && w.TemplateType==templateType)
                        .Select(x => new DO_DescriptiveResult
                        {
                            TemplateType = x.TemplateType,
                            DescriptiveResult = x.DescriptiveResult,
                        }
                        ).FirstOrDefaultAsync();
                    obj.DescriptiveResult = await descriptive;

                    return obj;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DO_ReturnParameter> AddOrUpdateServiceDescriptiveResult(DO_ServiceTemplateFull obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Update common values
                        var response = await AddOrUpdateServiceCommonValues(obj.CommonValues);
                        if (!response.Status)
                        {
                            return new DO_ReturnParameter() { Status = false, Message = response.Message };
                        }
                        // Update Descriptive Result 
                        var dobj = obj.DescriptiveResult;
                        var RecordExist = db.GtElasds.Where(w => w.BusinessKey == obj.CommonValues.BusinessKey && w.ServiceId == obj.CommonValues.ServiceId && w.TemplateType==obj.DescriptiveResult.TemplateType).FirstOrDefault();
                        if (RecordExist != null)
                        {
                            if (RecordExist.DescriptiveResult != dobj.DescriptiveResult)
                            {
                                RecordExist.DescriptiveResult = dobj.DescriptiveResult;

                                RecordExist.ModifiedBy = obj.CommonValues.UserID;
                                RecordExist.ModifiedOn = obj.CommonValues.CreatedOn;
                                RecordExist.ModifiedTerminal = obj.CommonValues.TerminalID;
                            }

                        }
                        else
                        {
                            var servicedescriptiveresult = new GtElasds
                            {
                                BusinessKey = obj.CommonValues.BusinessKey,
                                ServiceId = obj.CommonValues.ServiceId,
                                TemplateType = dobj.TemplateType,
                                DescriptiveResult = dobj.DescriptiveResult,

                                ActiveStatus = true,
                                FormId = obj.CommonValues.FormId,
                                CreatedBy = obj.CommonValues.UserID,
                                CreatedOn = obj.CommonValues.CreatedOn,
                                CreatedTerminal = obj.CommonValues.TerminalID
                            };
                            db.GtElasds.Add(servicedescriptiveresult);
                        }
                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        return new DO_ReturnParameter { Status = true };
                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        return new DO_ReturnParameter() { Status = false, Message = ex.Message };
                    }
                }
            }
        }
        #endregion
        #region CommonValues
        public async Task<DO_ReturnParameter> AddOrUpdateServiceCommonValues(DO_ServiceCommonValues obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var RecordExist = db.GtElascm.Where(w => w.BusinessKey == obj.BusinessKey && w.ServiceId == obj.ServiceId).FirstOrDefault();
                        if (RecordExist != null)
                        {
                            if (RecordExist.Interpretation != obj.Interpretation || RecordExist.Impression != obj.Impression || RecordExist.Note != obj.Note)
                            {
                                RecordExist.Interpretation = obj.Interpretation;
                                RecordExist.Impression = obj.Impression;
                                RecordExist.Note = obj.Note;

                                RecordExist.ModifiedBy = obj.UserID;
                                RecordExist.ModifiedOn = obj.CreatedOn;
                                RecordExist.ModifiedTerminal = obj.TerminalID;
                            }

                        }
                        else
                        {
                            var servicecommonvalue = new GtElascm
                            {
                                BusinessKey = obj.BusinessKey,
                                ServiceId = obj.ServiceId,
                                Interpretation = obj.Interpretation,
                                Impression = obj.Impression,
                                Note = obj.Note,

                                ActiveStatus = true,
                                FormId = obj.FormId,
                                CreatedBy = obj.UserID,
                                CreatedOn = obj.CreatedOn,
                                CreatedTerminal = obj.TerminalID
                            };
                            db.GtElascm.Add(servicecommonvalue);
                        }
                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        return new DO_ReturnParameter { Status = true };
                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        return new DO_ReturnParameter() { Status = false, Message = ex.Message };
                    }
                }
            }
        }
        public async Task<DO_ServiceCommonValues> GetServiceCommonValuesByBKeyServiceID(int businessKey, int serviceID)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtElascm.Where(w => w.BusinessKey == businessKey && w.ServiceId == serviceID)
                        .Select(x => new DO_ServiceCommonValues
                        {
                            Interpretation = x.Interpretation,
                            Impression = x.Impression,
                            Note = x.Note,
                        }
                        ).FirstOrDefaultAsync();
                    return await result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}

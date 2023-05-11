using eSyaLaboratory.DL.Entities;
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
    public class CommonMethodRepository : ICommonMethodRepository
    {
        public async Task<List<DO_BusinessLocation>> GetBusinessKey()
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    var bk = db.GtEcbsln
                        .Where(w => w.ActiveStatus)
                        .Select(r => new DO_BusinessLocation
                        {
                            BusinessKey = r.BusinessKey,
                            LocationDescription = r.BusinessName + " - " + r.LocationDescription
                        }).ToListAsync();

                    return await bk;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<DO_ApplicationCode>> GetApplicationCodesByCodeType(int codetype)
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    var bk = db.GtEcapcd
                        .Where(w => w.ActiveStatus && w.CodeType == codetype)
                        .Select(r => new DO_ApplicationCode
                        {
                            ApplicationCode = r.ApplicationCode,
                            CodeDesc = r.CodeDesc
                        }).ToListAsync();

                    return await bk;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<DO_ServiceClass>> GetServiceClasses(int servicetype)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtEssrcl
                        .Where(w=> w.ActiveStatus)
                        .Join(db.GtEssrgr,
                        c => c.ServiceGroupId,
                        g => g.ServiceGroupId,
                        (c, g) => new { c, g  })
                        .Where(w=> w.g.ServiceTypeId==servicetype)
                                 .Select(x => new DO_ServiceClass
                                 {
                                     ServiceClassId = x.c.ServiceClassId,
                                     ServiceClassDesc = x.c.ServiceClassDesc,
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
        public async Task<List<DO_ServiceCode>> GetLabServicesByBKey(int businessKey)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtEssrbl.Where(w => w.BusinessKey == businessKey)
                        .Join(db.GtEssrms,
                        b => b.ServiceId,
                        s => s.ServiceId,
                        (b, s) => new { b, s })
                        .Join(db.GtEssrcl,
                            bs => bs.s.ServiceClassId,
                            c => c.ServiceClassId,
                            (bs, c) => new { bs, c })
                        .Join(db.GtEssrgr.Where(w=> w.ServiceTypeId==1),
                            bsc =>  bsc.c.ServiceGroupId,
                            g => g.ServiceGroupId,
                            (bsc, g) => new { bsc, g })
                        
                        .Select(x => new DO_ServiceCode
                        {
                            ServiceId = x.bsc.bs.s.ServiceId,
                            ServiceDesc = x.bsc.bs.s.ServiceDesc,
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
    }
}

﻿using Microsoft.EntityFrameworkCore;
using SWP.Dtos.TreatmentPlan;
using SWP.Interfaces;
using SWP.Models;

namespace SWP.Repository
{
    public class TreatmentPlanRepository : ITreatmentPlan
    {
        private readonly HIEM_MUONContext _context;

        public TreatmentPlanRepository(HIEM_MUONContext context)
        {
            _context = context;
        }

        public async Task<TreatmentPlan?> CreateTreatmentPlan(TreatmentPlan treatmentPlan)
        {
            await _context.TreatmentPlans.AddAsync(treatmentPlan);
            await _context.SaveChangesAsync();
            var result = await GetTreatmentPlanById(treatmentPlan.TpId);
            return result;
        }

        public async Task<TreatmentStep?> CreateTreatmentStep(TreatmentStep treatmentStep)
        {
            await _context.TreatmentSteps.AddAsync(treatmentStep);
            await _context.SaveChangesAsync();
            var result = await GetTreatmentStepById(treatmentStep.TsId);
            return result;
        }

        public async Task<TreatmentPlan?> GetTreatmentPlanByCustomerId(int customerId)
        {
            return await _context.TreatmentPlans.Include(x => x.Doc).ThenInclude(x => x.Acc).Include(x => x.Ser).Include(x => x.Cus).Include(x => x.StatusNavigation).FirstOrDefaultAsync(x => x.CusId == customerId);

        }

        public async Task<TreatmentPlan?> GetTreatmentPlanById(int id)
        {
            return await _context.TreatmentPlans.Include(x => x.Doc).ThenInclude(x => x.Acc).Include(x => x.Ser).Include(x => x.Cus).Include(x => x.StatusNavigation).FirstOrDefaultAsync(x => x.TpId == id);
        }

        public async Task<TreatmentPlanStatus?> GetTreatmentPlanStatus(int id)
        {
            return await _context.TreatmentPlanStatuses.FirstOrDefaultAsync(x => x.StatusId == id);
        }

        public async Task<TreatmentStep?> GetTreatmentStepById(int id)
        {
            return await _context.TreatmentSteps.Include(x => x.Ser).FirstOrDefaultAsync(x => x.TsId == id);
        }

        public async Task<TreatmentPlan?> UpdateTreatmentPlan(int id, UpdateTreatmentPlanDto request)
        {
            var checkTreatmentPlan = await GetTreatmentPlanById(id);
            if (checkTreatmentPlan == null)
            {
                return null;
            }
            checkTreatmentPlan.SerId = request.SerId;
            checkTreatmentPlan.Status = request.Status;
            await _context.SaveChangesAsync();
            return checkTreatmentPlan;
        }

    }
}

using Psycho.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Psycho.Common.Domain;
using Psycho.Common.Domain.UserData;
using Serilog;

namespace Psycho.Service.Implementations
{
    public class OutcomeComputer : IOutcomeComputer
    {
        private ILogger _log;

        public OutcomeComputer(ILogger log)
        {
            _log = log;
        }

        public List<QuestionnaireOutcome> ComputeOutcomes(QuestionnaireChoices passedQuestionnaire, QuestionnaireScope scope)
        {
            if (passedQuestionnaire == null)
                return new List<QuestionnaireOutcome>();

            if (passedQuestionnaire.AnswerIds == null)
                return new List<QuestionnaireOutcome>();

            if (passedQuestionnaire.AnswerIds.Count == 0)
                return new List<QuestionnaireOutcome>();

            if (passedQuestionnaire.ScopeId != scope.Id)
                return new List<QuestionnaireOutcome>();

            _log.Information($"Respondent results:{passedQuestionnaire}");

            var mappings = scope.Entries
                .SelectMany(z => z.Answers)
                .Where(z => passedQuestionnaire.AnswerIds.Contains(z.Id))
                .SelectMany(z => z.Mappings)
                .GroupBy(z => z.OutcomeIndex)
                .Select(z => new { O = z.Key, W = z.Sum(x => x.Weight) })
                .OrderBy(z => -z.W)
                .ToArray();

            var res = new List<QuestionnaireOutcome>();
            foreach (var map in mappings)
                if (map.W > 0)
                {
                    var outc = scope.Outcomes.Where(z => z.Index == map.O && Match(z, map.W));
                    if (outc.Any())
                    {
                        res.AddRange(outc);
                    }
                }

            if (!res.Any())
            {
                var cand = scope.Outcomes.FirstOrDefault(z => z.WeightMin != z.WeightMax && z.WeightMin == 0);
                if (cand != null)
                {
                    _log.Information($"Return default outcome {cand}");
                    return new List<QuestionnaireOutcome> { cand };
                }
            }
            _log.Information($"Oucomes: {string.Join("; ", res)}");
            if (scope.OutcomeLimit == 0)
                return res.ToList();
            else
                return res.Take(scope.OutcomeLimit).ToList();
        }

        private bool Match(QuestionnaireOutcome outcome, double weigth)
        {
            if (outcome.WeightMin == outcome.WeightMax)
                return true;
            return outcome.WeightMin <= weigth && outcome.WeightMax >= weigth;
        }
    }
}

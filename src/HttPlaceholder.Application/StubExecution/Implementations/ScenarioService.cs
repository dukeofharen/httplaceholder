// TODO move to StubContext
// using System.Collections.Generic;
// using System.Threading;
// using System.Threading.Tasks;
// using HttPlaceholder.Application.Infrastructure.DependencyInjection;
// using HttPlaceholder.Application.Interfaces.Signalling;
// using HttPlaceholder.Domain;
// using HttPlaceholder.Domain.Entities;
//
// namespace HttPlaceholder.Application.StubExecution.Implementations;
//
// internal class ScenarioService : IScenarioService, ISingletonService
// {
//     private readonly IScenarioNotify _scenarioNotify;
//     private readonly IScenarioStateStore _scenarioStateStore;
//
//     public ScenarioService(IScenarioStateStore scenarioStateStore, IScenarioNotify scenarioNotify)
//     {
//         _scenarioStateStore = scenarioStateStore;
//         _scenarioNotify = scenarioNotify;
//     }
//
//     /// <inheritdoc />
//     public async Task IncreaseHitCountAsync(string scenario, CancellationToken cancellationToken)
//     {
//         if (string.IsNullOrWhiteSpace(scenario))
//         {
//             return;
//         }
//
//         ScenarioStateModel scenarioState;
//         lock (_scenarioStateStore.GetScenarioLock(scenario))
//         {
//             scenarioState = GetOrAddScenarioState(scenario, out _);
//             scenarioState.HitCount++;
//             _scenarioStateStore.UpdateScenario(scenario, scenarioState);
//         }
//
//         await _scenarioNotify.ScenarioSetAsync(scenarioState, cancellationToken);
//     }
//
//     /// <inheritdoc />
//     public async Task<int?> GetHitCountAsync(string scenario, CancellationToken cancellationToken)
//     {
//         if (string.IsNullOrWhiteSpace(scenario))
//         {
//             return null;
//         }
//
//         ScenarioStateModel scenarioState;
//         bool scenarioAdded;
//         lock (_scenarioStateStore.GetScenarioLock(scenario))
//         {
//             scenarioState = GetOrAddScenarioState(scenario, out scenarioAdded);
//         }
//
//         if (scenarioAdded)
//         {
//             await _scenarioNotify.ScenarioSetAsync(scenarioState, cancellationToken);
//         }
//
//         return scenarioState.HitCount;
//     }
//
//     /// <inheritdoc />
//     public IEnumerable<ScenarioStateModel> GetAllScenarios() => _scenarioStateStore.GetAllScenarios();
//
//     /// <inheritdoc />
//     public ScenarioStateModel GetScenario(string scenario) => _scenarioStateStore.GetScenario(scenario);
//
//     /// <inheritdoc />
//     public async Task SetScenarioAsync(string scenario, ScenarioStateModel scenarioState,
//         CancellationToken cancellationToken)
//     {
//         if (string.IsNullOrWhiteSpace(scenario) || scenarioState == null)
//         {
//             return;
//         }
//
//         lock (_scenarioStateStore.GetScenarioLock(scenario))
//         {
//             var existingScenario = _scenarioStateStore.GetScenario(scenario);
//             if (existingScenario == null)
//             {
//                 if (string.IsNullOrWhiteSpace(scenarioState.State))
//                 {
//                     scenarioState.State = Constants.DefaultScenarioState;
//                 }
//
//                 _scenarioStateStore.AddScenario(scenario, scenarioState);
//             }
//             else
//             {
//                 if (scenarioState.HitCount == -1)
//                 {
//                     scenarioState.HitCount = existingScenario.HitCount;
//                 }
//
//                 if (string.IsNullOrWhiteSpace(scenarioState.State))
//                 {
//                     scenarioState.State = existingScenario.State;
//                 }
//
//                 _scenarioStateStore.UpdateScenario(scenario, scenarioState);
//             }
//         }
//
//         await _scenarioNotify.ScenarioSetAsync(scenarioState, cancellationToken);
//     }
//
//     /// <inheritdoc />
//     public async Task<bool> DeleteScenarioAsync(string scenario, CancellationToken cancellationToken)
//     {
//         if (string.IsNullOrWhiteSpace(scenario))
//         {
//             return false;
//         }
//
//         bool result;
//         lock (_scenarioStateStore.GetScenarioLock(scenario))
//         {
//             result = _scenarioStateStore.DeleteScenario(scenario);
//         }
//
//         await _scenarioNotify.ScenarioDeletedAsync(scenario, cancellationToken);
//         return result;
//     }
//
//     /// <inheritdoc />
//     public async Task DeleteAllScenariosAsync(CancellationToken cancellationToken)
//     {
//         _scenarioStateStore.DeleteAllScenarios();
//         await _scenarioNotify.AllScenariosDeletedAsync(cancellationToken);
//     }
//
//     private ScenarioStateModel GetOrAddScenarioState(string scenario, out bool scenarioAdded)
//     {
//         scenarioAdded = false;
//         var scenarioModel = _scenarioStateStore.GetScenario(scenario);
//         if (scenarioModel == null)
//         {
//             scenarioModel = _scenarioStateStore.AddScenario(
//                 scenario,
//                 new ScenarioStateModel(scenario));
//             scenarioAdded = true;
//         }
//
//         return scenarioModel;
//     }
// }

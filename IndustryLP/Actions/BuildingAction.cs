﻿using ColossalFramework.UI;
using IndustryLP.DomainDefinition;
using IndustryLP.Entities;
using IndustryLP.UI.Panels;
using IndustryLP.Utils;
using IndustryLP.Utils.Enums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IndustryLP.Actions
{
    internal class BuildingAction : ToolAction
    {
        #region Attributes

        private IMainTool m_mainTool = null;
        private UIGeneratorOptionPanel m_panel = null;
        private BuildThread m_generator = null;
        private int m_rows;
        private int m_columns;

        #endregion

        #region Properties

        public GenerationState CurrentState { get; set; } = GenerationState.None;

        #endregion

        #region Action Behaviour methods

        public override void OnStart(IMainTool mainTool)
        {
            base.OnStart(mainTool);
            m_mainTool = mainTool;
        }

        public override void OnEnterController()
        {
            base.OnEnterController();
            m_rows = m_mainTool.Distribution.Rows;
            m_columns = m_mainTool.Distribution.Columns;
            m_panel = SetupPanel();
            m_generator = SetupBuildThread();
            CurrentState = GenerationState.GeneratingSolutions;
        }

        public override void OnLeftController()
        {
            base.OnLeftController();
            Object.DestroyImmediate(m_panel.gameObject);
            m_generator.Stop();
            Object.DestroyImmediate(m_generator.gameObject);
            CurrentState = GenerationState.None;
        }

        public override void OnUpdate(Vector3 mousePosition)
        {
            base.OnUpdate(mousePosition);

            if (CurrentState == GenerationState.GeneratingSolutions)
            {
                m_panel.SetSolutions(m_generator.Count);

                if (m_generator.IsFinished)
                {
                    CurrentState = GenerationState.GeneratedSolutions;
                    m_panel.StopLoading();
                }
            }
        }

        #endregion

        #region Private methods

        private UIGeneratorOptionPanel SetupPanel()
        {
            var panel = GameObjectUtils.AddUIComponent<UIGeneratorOptionPanel>();
            panel.relativePosition = new Vector3(791, 847); ;
            panel.OnClickPreviousSolution += OnChangeSolution;
            panel.OnClickNextSolution += OnChangeSolution;
            return panel;
        }

        private BuildThread SetupBuildThread()
        {
            var thread = GameObjectUtils.AddObjectWithComponent<BuildThread>();
            thread.StartProgram(64, m_rows, m_columns, ConvertTo(m_mainTool.Preferences), ConvertTo(m_mainTool.Restrictions));
            return thread;
        }

        private void OnChangeSolution(UIComponent component, UIMouseEventParameter eventParameter)
        {
            LoggerUtils.Log($"Solution {m_panel.Solution}: ");
            var solution = m_generator.GetSolution(m_panel.Solution - 1);

            for (int i = 0; i < m_rows; i++)
            {
                for (int j = 0; j < m_columns; j++)
                {
                    string buildingName = solution.Parcels[i, j];
                    LoggerUtils.Log($"[{i}, {j}] = {buildingName}");
                }
            }
        }

        private List<BuildingAtom> ConvertTo(List<Parcel> parcels)
        {
            return parcels.Select(parcel => {
                var position = m_mainTool.Distribution.GetGridPosition(parcel.GridId);
                return new BuildingAtom { Row = position.First, Column = position.Second, Name = parcel.Building.name };
            }).ToList();
        }

        #endregion
    }
}
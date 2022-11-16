//-------------------------------------------------------------------------
// <copyright file="Recipe.cs" company="Universidad Católica del Uruguay">
// Copyright (c) Programación II. Derechos reservados.
// </copyright>
//-------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Full_GRASP_And_SOLID
{
    public class Recipe : IRecipeContent // Modificado por DIP
    {
        // Cambiado por OCP
        protected IList<BaseStep> steps = new List<BaseStep>();
        private TimerAdapter timerClient;
        private CountdownTimer timer = new CountdownTimer();
        public bool Cooked { get; private set; } = false;

        public void Cook()
        {
            if (this.Cooked)
            {
                InvalidOperationException e = new InvalidOperationException();
                throw e;
            }
            StartCountDown();
        }
        public Product FinalProduct { get; set; }

        // Agregado por Creator
        public void AddStep(Product input, double quantity, Equipment equipment, int time)
        {
            Step step = new Step(input, quantity, equipment, time);
            this.steps.Add(step);
        }

        // Agregado por OCP y Creator
        public void AddStep(string description, int time)
        {
            WaitStep step = new WaitStep(description, time);
            this.steps.Add(step);
        }

        public void RemoveStep(BaseStep step)
        {
            this.steps.Remove(step);
        }

        // Agregado por SRP
        public string GetTextToPrint()
        {
            string result = $"Receta de {this.FinalProduct.Description}:\n";
            foreach (BaseStep step in this.steps)
            {
                result = result + step.GetTextToPrint() + "\n";
            }

            // Agregado por Expert
            result = result + $"Costo de producción: {this.GetProductionCost()}";

            return result;
        }

        // Agregado por Expert
        public double GetProductionCost()
        {
            double result = 0;

            foreach (BaseStep step in this.steps)
            {
                result = result + step.GetStepCost();
            }

            return result;
        }
        public int GetCookTime()
        {
            int TotalTime = 0;
            foreach (BaseStep step in this.steps)
            {
                TotalTime += step.Time;
            }
            return TotalTime;
        }
        private void StartCountDown()
        {
            this.timerClient = new TimerAdapter(this);
            this.timer.Register(GetCookTime(), this.timerClient);
        }
        private class TimerAdapter : TimerClient
        {
            private Recipe recipe;
            public TimerAdapter(Recipe recipe)
            {
                this.recipe = recipe;

            }
            public void TimeOut()
            {
                this.recipe.Cooked = true;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections;

namespace ACT4 {
    public class GeneticAlgo {
        private int groupSize = 100;
        private double mutationChance = 0.05;
        private int maxGenerations = 1000;
        private int boardSize = 6;
        private Random rng = new Random();

        public SixState SolveWithGenetics() {
            List<SixState> myPopulation = CreateStartingGroup();
            SixState topCandidate = null;

            for (int gen = 0; gen < maxGenerations; gen++) {
                myPopulation = myPopulation.OrderBy(state => CountConflicts(state)).ToList();
                topCandidate = myPopulation[0];

                if (CountConflicts(topCandidate) == 0)
                    break;

                List<SixState> newGroup = new List<SixState>();

                for (int i = 0; i < groupSize / 2; i++) {
                    SixState parentA = PickParent(myPopulation);
                    SixState parentB = PickParent(myPopulation);
                    SixState kid1, kid2;
                    MixParents(parentA, parentB, out kid1, out kid2);

                    Mutate(kid1);
                    Mutate(kid2);

                    newGroup.Add(kid1);
                    newGroup.Add(kid2);
                }

                myPopulation = newGroup;
            }

            return topCandidate;
        }

        private List<SixState> CreateStartingGroup() {
            List<SixState> population = new List<SixState>();

            for (int i = 0; i < groupSize; i++) {
                SixState state = new SixState();
                for (int j = 0; j < boardSize; j++) {
                    state.Y[j] = rng.Next(boardSize); // queen rng
                }
                population.Add(state);
            }

            return population;
        }

        private int CountConflicts(SixState state) {
            int totalConflicts = 0;

            for (int i = 0; i < boardSize; i++) {
                for (int j = i + 1; j < boardSize; j++) {
                    if (state.Y[i] == state.Y[j] || 
                        state.Y[i] == state.Y[j] + (j - i) || 
                        state.Y[i] == state.Y[j] - (j - i)) 
                    {
                        totalConflicts++;
                    }
                }
            }

            return totalConflicts;
        }

        private SixState PickParent(List<SixState> group) {
            int overallFitness = group.Sum(state => 1 / (1 + CountConflicts(state)));
            int chosenValue = rng.Next(overallFitness);
            int fitnessCounter = 0;

            foreach (SixState state in group) {
                fitnessCounter += 1 / (1 + CountConflicts(state));
                if (fitnessCounter >= chosenValue)
                    return state;
            }

            return group[0];
        }

        private void MixParents(SixState parent1, SixState parent2, out SixState child1, out SixState child2) {
            child1 = new SixState();
            child2 = new SixState();
            int swapPoint = rng.Next(boardSize);

            for (int i = 0; i < boardSize; i++) {
                if (i < swapPoint) {
                    child1.Y[i] = parent1.Y[i];
                    child2.Y[i] = parent2.Y[i];
                } else {
                    child1.Y[i] = parent2.Y[i];
                    child2.Y[i] = parent1.Y[i];
                }
            }
        }

        private void Mutate(SixState state) {
            if (rng.NextDouble() < mutationChance) {
                int index = rng.Next(boardSize);
                state.Y[index] = rng.Next(boardSize); // queen pos rng
            }
        }
    }
}
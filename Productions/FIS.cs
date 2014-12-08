using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;//to enable debug. delete in final version
using System.Math;
namespace Productions
{
    class FIS
    {
        public   IDictionary<String,Variable> Inputs;// = new Dictionary<String,Variable>();
        public   IDictionary<String, Variable> Outputs;// = new Dictionary<String, Variable>();
        public   List<Production> Productions;// = new List<Production>();
        public   List <Parameter>Parameters;//=new List <Parameter>();
//parameters as dictionary
//        public IDictionary<String,Parameter>;

  public FIS(IDictionary<string, Variable> InVars, IDictionary<string, Variable> OutVars, List<Production> Prods, List<Parameter> Params)
      //IDictionary <string, Parameter> Params)
  {
            this.Inputs = InVars;
            this.Outputs = OutVars;
            this.Productions = Prods;
            this.Parameters = Params;
            //зададим коллекцию Мvalues
            CreateMFValues();
            double h = 0;
            foreach (Variable Y in this.Outputs.Values)
            {
                h = (Y.RightB - Y.LeftB) / 100;
                for (int i = 0; i < 100; i++)
                {
                    MFValue mf = new MFValue(Y.LeftB + i * h);
                    Y.MFValues.Add(mf);
                }
            }//foreach
}//constructor
  public void CreateMFValues()
  {
      int i;
      double h;
      MFValue mv;
      foreach (Variable y in this.Outputs.Values)
      {
          h = (y.RightB - y.LeftB) / 100;
          for (i = 0; i < 100; i++)
          {
              mv = new MFValue(y.LeftB + i * h);//argument
              y.MFValues.Add(mv);
          }//inner for  
      }//foreach
  }//CreateMFValues


//Methods----------------------------------------------
  int getId(string name, List<Parameter> Param) {
      int i = 0;
      while (i < Param.Count())
      {
          if (Param[i].Name == name) return i;
          else
          {
              System.Windows.Forms.MessageBox.Show("ERR: WRONG PARAM NAME");
              return 0;
          }
          i++;
      } return -1;
  }

        
       

//AND      
        public double FuzzyAnd(double x, double y){
            int ind = this.getId("FAndType", this.Parameters);
 
         //IDictionary variant
        //    KeyValuePair<"FAndType",Parameters>;

            switch (this.Parameters[ind].Name){
                case "Min":
                 return  Math.Min(x, y);
                case "Prod":
                    return x*y;
                 default: System.Windows.Forms.MessageBox.Show("ERR: WRONG VALUE");return 0;
                }//switch
        }//fuzzyAnd


        //*/
            /* //first variant    
                public Double FuzzyAnd(Double Value,Double LValue)
                {
                    return Math.Max(Value,LValue);
                }
             */

//OR
            ///*
        public double FuzzyOr(double x, double y){
        int ind = this.getId("FOrType", this.Parameters);    

            switch(this.Parameters[ind].Name){
                case "Max":
                    return Math.Max(x, y);
                case "Sum":
                    return x + y - x * y;
                default:  System.Windows.Forms.MessageBox.Show("ERR: WRONG VALUE");return 0;
           }//switch
        }//fuzzyOr
          //   */


        /* //one more fuzzyOr
        public Double FuzzyOr(Double Value, Double LValue)
        {
            return Math.Min(Value, LValue);
        }
         //*/
    
        
        
        public void Agregation()//Double Value, Double LValue)
        { 
            foreach (Production P in this.Productions){
                Debug.Print("aggregation is on.");
                P.Antecedent.Calc(this);}

         }//Agregation


        public void Activation()//(Double Value, Double LValue)
        {Debug.Print("activation is on.");
            foreach (Production P in this.Productions){
                P.Consequent.Value = this.FuzzyAnd(P.Antecedent.Value, P.BF);}
        }//Activation

        
        public void Accumulation()//(Double Value, Double LValue)
        {
            foreach (Variable Y in this.Outputs.Values){//1
                foreach (LTerm L in Y.LTerms.Values){//2
                    L.Value = 0;
                      foreach(Production P in this.Productions){//3
                          if ((P.Consequent.LTName == L.Name) && (P.Consequent.VarName == Y.Name)){
                              L.Value = this.FuzzyOr(L.Value, P.Consequent.Value);
                            }//if
                        }//foreach 3
                    }//foreach 2
                }// foreach 1
        }//Accumulation


        public void CutAndUnion() //double
        {foreach (Variable Y in this.Outputs.Values){//1
                foreach (MFValue M in Y.MFValues) //предварительно для мф задать размер в конструкторе (выходных переменных)
                {//2
                   M.Value = 0;  //во избежание ошибок
                    foreach (LTerm L in Y.LTerms.Values){//3
                              M.Value = this.FuzzyOr(M.Value, this.FuzzyAnd(L.MF.Calc(M.Arg), L.Value)); 
                         }//foreach 3
                }//foreach 2
            }//foreach 1
         }//CutAndUnion
 ////FUZZ==========================================================
        public void Fuzzification()
        {//(IDictionary Inputs){
           // MFValue MFV=new MFValue(0);
            foreach (KeyValuePair<string, Variable> X in this.Inputs){//1
              foreach (LTerm L  in X.Value.LTerms.Values){//2
                        Debug.Print("LTerm = " + L.Name);
                        L.Value=L.MF.Calc(X.Value.Value);
                //MFV=new MFValue(X.Value.Name,L.MFunction(X.Value).Value);
               // X.Value.MFValues.Add(MFV);
                }//foreach 2                
            }//foreach 1
          }//Fuzzification
////================================================================
        public void Defuzzification()// значение итоговой функции - мы пока будем считать только по центроидному методу
        {foreach (Variable Y in this.Outputs.Values){
                Y.Value = DeFuzzMeth(Y.MFValues);}
        }

         public double DeFuzzMeth(List<MFValue>MValues)//должна быть дискретная дефаззификация через суммы и дробь
        {//Centroid
            return Centroid(MValues);
        }
// --fuzzification methods----------------------------------------------
        

        double Max(double x)//, List<Parameter> Params)
        {
        double z;
        int i;
        z = x;
            for (i =0;i<this.Parameters.Count();i++) 
            {
                if (this.Parameters.ElementAt(i).Value > z)
                {
                    z = this.Parameters.ElementAt(i).Value;
                }
            }
            //Params.Min();i=Params.Max(); i++)
           return z;
         }//Max

        double Min(double x)//, List<Parameter> Params)
        {
            double z;
            int i;
            z = x;
            for (i = 0; i < this.Parameters.Count(); i++)
            {
                if (this.Parameters.ElementAt(i).Value < z)
                {
                    z = this.Parameters.ElementAt(i).Value; 
                }
            }
            return z;
         }//Min end

        // /* 
        double Centroid(List<MFValue>MVals) {
         double n, d;   n = 0; d = 0;

           foreach(MFValue m in MVals){
               n = n + m.Arg + m.Value;
               d = d + m.Value;}
        return n / d;
      }//centroid


        /*
        public double Centroid(List<MFValue>MVals)
        {
        double Num=0, Den=0;
          
    foreach (MFValue M in MVals)
    {
        Num = Num + M.Arg + M.Value;
        Den = Den + M.Value;
       // Num = Num + x(i) * i;
        //Den = Den + x(i);
         
         }//foreach
    return Num / Den;
            
        }
        //*/


    }//FIS
}

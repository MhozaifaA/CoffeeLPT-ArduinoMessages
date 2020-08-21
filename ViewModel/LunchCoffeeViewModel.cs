using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CoffeeLPT.Uitl.MVVM;

namespace CoffeeLPT.ViewModel
{
    public class LunchCoffeeViewModel : ViewModelBase
    {
        #region Init
        [DllImport("inpout32.dll")]
        private static extern UInt32 IsInpOutDriverOpen();
        [DllImport("inpout32.dll")]
        private static extern void Out32(short PortAddress, short Data);
        [DllImport("inpout32.dll")]
        private static extern byte Inp32(short PortAddress);

        [DllImport("inpoutx64.dll", EntryPoint = "IsInpOutDriverOpen")]
        private static extern UInt32 IsInpOutDriverOpen_x64();
        [DllImport("inpoutx64.dll", EntryPoint = "Out32")]
        private static extern void Out32_x64(short PortAddress, short Data);
        [DllImport("inpoutx64.dll", EntryPoint = "Inp32")]
        private static extern char Inp32_x64(short PortAddress);

      
        
        DispatcherTimer timer = new DispatcherTimer();
          private Task TaskOrder=new Task(() => {});
             IEnumerable <Order> OrderButtons=new List<Order>()
          {
              new Order()
              {
                  Name = "Coffee",
                  Time = 1,
                  Temp = 10,
              },
              new Order()
              {
                  Name = "3 in 1",
                  Time = 2,
                  Temp = 5,
              },
              new Order()
              {
                  Name = "cappuccino",
                  Time = 3,
                  Temp = 15,
              },
          };

           //TODO  private byte Data = 0;
          //TODO List<Byte> D3D4D5List=new List<byte>();
        public LunchCoffeeViewModel()
        {
            CommandCloseWindow=new CommandT<Window>(window =>
            {
                window.Close();
            });
            _OrderList=new ObservableCollection<Order>();
            TimerTemp();

            //TODO Output Out32(0x37A, 0x00); 
            /*
             تغير القطب الخامس إلى 0 ليصبح الداتا للخرج
             */
            //ToDo Data Out32(0x378, 0x00); 
            // تهيئة قيم صفرية للداتا
            //  Data=0;
        }

        
        #endregion

        #region Commands

        public ICommand CommandCloseWindow { get; }


        private ICommand _CommandButtonDrinks;
        public ICommand CommandButtonDrinks
        {
            get
            {
                return CommandButtonDrinks = new CommandT<double>(kind =>
                {
                    //byte DataInPut = Inp32(0x379);
                    //TODo if(((DataInPut & 0x08) == 0x08) && (DataInPut & 0x10) == 0x10) // I3=1 && I4 =1
                    // {
                         _OrderList.Add(OrderButtons.ToList()[(int) kind - 1]);
                        OnPropertyChanged(nameof(CommandStartOrder));
                    // إضافة إلاليست الطلبات الداتا قيم ال إخراج
                    // 
                    // if(kind==1)
                    // {

                          //  D3=1 وضع داتا D4=0 D5=0
                          // ToDo D3D4D5List.Add(0x08);

                    // }
                    // if(kind==2)
                    // {

                           //  D4=1 وضع داتا D3=0 D5=0
                           // ToDo D3D4D5List.Add(0x10);

                    // }
                    // if(kind==3)

                          //  D5=1 وضع داتا  D3=0 D4=0
                          // ToDo D3D4D5List.Add(0x20);

                    //  }


                }, d => { return _WaterList.Any() && _CupList.Any(); } );
            }
            set { _CommandButtonDrinks = value; }
        }


        private ICommand _CommandFillWater;
        public ICommand CommandFillWater
        {
            get { return _CommandFillWater=new Command(async () =>
            {

                //TODO   byte DataInPut = Inp32(0x379);

                /* I3=0  خزان مياه : فارغ
                   I3=1  مليان
                    لنفحص إذا فارغ لنقم بتعبئته
                    ملاحظة : يكون مليئ إذا كان يحوي منسوب واحد على الأقل
                    و فارغ إذالا يحوي أي منسوب  
                    */
                //ToDo Input if ((DataInPut & 0x08) == 0x00)   // فارغ if I3=0  enter if else 0x08 
                //{

                /* D0=1 بدء تعبئة الخزان : عند إعطاء */
                //  Data |=0x01;
                //  byte D0To1 =Data;
                //TODO Output-Data Out32(0x378, D0To1); //  D0=1 مع الحفاظ على القيم  

                for (int i = _WaterList.Count; i < 10; ++i)
                   {
                       await Task.Run(async () =>
                       {
                           await Task.Delay(500);
                           await App.Current.Dispatcher.InvokeAsync((Action) delegate
                           {
                               if (_WaterList.Count < 10)
                                   _WaterList.Add("");
                           });
                           OnPropertyChanged(nameof(CommandButtonDrinks));

                       });
                   }

                /* D0=0 وقف تعبئة الخزان : عند إعطاء */
                //  Data &=0xFE;
                //  byte D0To0 =Data;
                //TODO Output-Data Out32(0x378, D0To0); //  D0=1 مع الحفاظ على القيم  


                // }
                // else    if ((DataInPut & 0x08) == 0x08)   مليان  if I3=1
                // { 
                // لايمكن تعبئة الخزان لأنه ممتلئ
                //  }
            }); }
            set { _CommandFillWater = value; }
        }

     

        private ICommand _CommandFillCup;
        public ICommand CommandFillCup
        {
            get
            {
                return _CommandFillWater = new Command(async () =>
                {
                    //TODO   byte DataInPut = Inp32(0x379);

                    /* I4=0  خزان الكاسات : فارغ
                       I4=1  مليان
                        لنفحص إذا فارغ لنقم بتعبئته
                       ملاحظة : يكون مليئ إذا كان يحوي منسوب واحد على الأقل
                      و فارغ إذالا يحوي أي منسوب  
                    */
                    //ToDo Input if ((DataInPut & 0x10) == 0x00)   // فارغ if I4=0  enter if else 0x10 
                    //{

                    /* D1=1 بدء تعبئة الكاسات : عند إعطاء */
                    //  Data |=0x02;
                    //  byte D1To1 =Data;
                    //TODO Output-Data Out32(0x378, D1To1); //  D1=1 مع الحفاظ على القيم  

                    for (int i = _CupList.Count; i < 10; ++i)
                    {
                        await Task.Run(async () =>
                        {
                            await Task.Delay(500);
                            await App.Current.Dispatcher.InvokeAsync((Action)delegate
                            {
                                if(_CupList.Count<10)
                                _CupList.Add("");
                            });
                            OnPropertyChanged(nameof(CommandButtonDrinks));


                        });
                    }


                    /* D1=0 وقف تعبئة الكاسات : عند إعطاء */
                    //   Data &=0xFD;
                    // byte D1To0 = Data;
                    //TODO Output-Data Out32(0x378, D0To0); //  D0=1 مع الحفاظ على القيم  


                  

                    // }
                    // else    if ((DataInPut & 0x10) == 0x10)   مليان  if I4=1
                    // { 
                    // لايمكن تعبئة الكاسات لأنه ممتلئ
                    //  }
                });
            }
            set { _CommandFillWater = value; }
        }

      

        private ICommand _CommandStartOrder;
        public ICommand CommandStartOrder
        {
            get { return _CommandStartOrder=new Command(async () =>
            {

                //TODO byte _DataInPut = Inp32(0x379);

                /* يجب التأكد أن لايكون ال طلبات فارغ
                  I5=1 يوجد طلب*/
                // TODO if((_DataInPut &  0x20)==0x20)
                /////**ignor**////
                
                /*
                 D2=1 تشغيل الطلبات
                 */
                // ToDo Data|=0x04;
                // Todo Out32(0x378,Data);

                if (TaskOrder.Status != TaskStatus.Running && TaskOrder.Status!= TaskStatus.WaitingForActivation)
                    foreach (var order in _OrderList.ToList())
                    {
                        //TODO foreach (var Dataorder in D3D4D5List)

                        //TODO byte DataInPut = Inp32(0x379); // كل دورة التحقق

                        if (_ValueTemp >= 40 && _CupList.Count>0 && _WaterList.Count>0) 
                              // ToDO &&  if(((DataInPut & 0x08) == 0x08) && (DataInPut & 0x10) == 0x10)  // يجب أن لا يكون أحد الخزانات فارغ
                        {       
                               TaskOrder=  Task.Run(async () =>
                              {
                                  await Task.Delay((int)order.Time * 1000);
                                  await App.Current.Dispatcher.InvokeAsync( (Action) delegate
                                  {
                                      _OrderList.Remove(order);
                                      _WaterList.RemoveAt(0);
                                      _CupList.RemoveAt(0);
                                  });
                                  _ValueTemp -= order.Temp;
                                  OnPropertyChanged(nameof(ValueTemp));

                              });
                              await TaskOrder;


                            // TODO    D3=1 وضع داتا D4=0 D5=0

                                   // ترسيت D3=0 , D4=0 , D5=0
                                   //TODO reset// Data &=0x0C7
                                   // TODo Data |=Dataorder
                                   // TODO Out32(0x378,Data);
                                   // D3D4D5List.Remove(Dataorder);

                        }
                        //else
                        //{

                        // هنا يمكن اضافة تعبئة عبر التحكم ب
                        // D0=1, D1=1

                        //TODO reset// Data &=0C7
                        // TODO Out32(0x378,Data);

                        // ايقاف الطلبات
                        // TODo Data&=0xFB
                        // Todo Out32(0x378, Data);
                        //}

                        //else
                        //{

                        //** يكون عندها الطلبات فارغ I5=0

                        //TODO reset// Data &=0C7
                        // TODO Out32(0x378,Data);

                        //}
                    }


            }, () => { return _OrderList.Any() && _ValueTemp >= 40 && 
                              TaskOrder.Status != TaskStatus.Running && TaskOrder.Status != TaskStatus.WaitingForActivation;
            }); }
            set { _CommandStartOrder = value; }
        }



        #endregion




        #region Method

        void TimerTemp()
        {
            
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += new EventHandler((sender, args) =>
            {
                // ToDo byte DataInPut = Inp32(0x379);

                /*
                 تكون السخان متوقف عندما تكون درجة الحرارة ‘أعلى من 70
                     I6   نفرض أن 
                     حيث I6=1 أقل من 70
                     , I6=0 عند ال 70
                 * */
                // ToDo if(( DataInput & 0x40)==0x40) 
                // {
                //D6=1 رقع درجة الحرارة عند D6
                //
                // Data |=0x40
                //ToDo Out32(0x378,Data);

                if (_ValueTemp < 70)
                {
                    _ValueTemp++;
                    OnPropertyChanged(nameof(ValueTemp));
                    OnPropertyChanged(nameof(CommandStartOrder));
                }

                //  else {
                // عن طريق إطفاء السخان
                // D6=0
                //  // Data |=0x7F
                //  ToDo Out32(0x378,Data);
                //}
                //}


                //  التأكد من حالة الخزان انه فارغ لملئه
                // byte DataInput=Input32(0x379);
                // ToDo if((DataInout & 0x08)==0x00)
                // Data |=0x01;
                /// Out32(0x378,Data);


                if(!WaterList.Any())
                for (int i = WaterList.Count; i < 10; ++i)
                {
                    if (WaterList.Count < 10)
                        WaterList.Add("");
                }




            });
            timer.Start();
        }



        #endregion


        #region InnerClass

        public class Order
        {
            public string Name { get; set; }
            public double Time { get; set; }
            public int Temp { get; set; }
        }

        #endregion

        #region List

        private ObservableCollection<object> _WaterList=new ObservableCollection<object>();
        public ObservableCollection<object> WaterList
        {
            get { return _WaterList; }
            set { _WaterList = value; }
        }


        private ObservableCollection<object> _CupList=new ObservableCollection<object>();
        public ObservableCollection<object> CupList
        {
            get { return _CupList; }
            set { _CupList = value; }
        }


        private ObservableCollection<Order> _OrderList;
        public ObservableCollection<Order> OrderList
        {
            get { return _OrderList; }
            set { _OrderList = value; }
        }


        #endregion


        #region Number

        private int _ValueTemp;
        public int ValueTemp
        {
            get
            {
                //if (_ValueTemp > 70)
                //{
                //    timer.Stop();
                //}
                return _ValueTemp;
            }
            set
            {
                _ValueTemp = value;
            }
        }


        #endregion



    }
}

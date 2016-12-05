using Model.DataModels;

namespace Model
{

     namespace DataContainer
    {
        public class Data
        {
            private static Data instance;
            private Data() { }

            public static Data Instance
            {
                get
                {
                    if (instance == null)
                    {
                        instance = new Data();
                    }

                    return instance;
                }
            }
            public ElementContainer [] elementList;
            public int currentId;

            public DataModels.OutpueEnum getResult(int id)
            {
                foreach (ElementContainer element in this.elementList)
                {
                    if (element.id == id)
                    {
                        return element.state;
                    }
                }

                return OutpueEnum.unknown;
            }

        }

        public struct ElementContainer
        {

            public int id;
            public int order;
            public bool dirty
            {
                get { return this.dirty; }
                set
                {
                    this.dirty = value;
                    if (this.dirty == true)
                    {
                        elem.CalculateOutput(this.id);
                    }
                    else
                    {

                    }
                }

            }
            public Elements.Element elem;
            public int [] linkedElem;

            public DataModels.OutpueEnum state;
            public DataModels.ElementType elementType;
            public DataModels.GateType gateType;
            public DataModels.RawType rawType;

            public int X1;
            public int X2;
            public int Y1;
            public int Y2;
            public bool movable;
            public int inputNumber;
        }
    }

    namespace DataModels
    {
        public enum OutpueEnum
        {
            low = 0,
            high = 1,
            unknown = -1
        }

        public enum GateType
        {
            AND, NAND, OR, NOR, NOT
        }


        public enum RawType
        {
            OutputPoint, Junction, InBus
        }
        public enum ElementType
        {
            gateType, rawType
        }
    }

    namespace Elements
    {
        public class Element
        {
            public  int recurenceCalculation(int inputNumber, int[] outputStates, GateType gateType)
            {
                if (gateType != GateType.NOT)
                {
                    int[] newOutputStates = new int[inputNumber / 2 + inputNumber % 2];
                    int result = -1;

                    int counter = 0;

                    for (int i = 0; i < (inputNumber / 2 - inputNumber % 2); i = i + 2)
                    {
                        switch (gateType)
                        {
                            case GateType.AND:
                                newOutputStates[counter] = outputStates[i] * outputStates[i + 1];
                                break;

                            case GateType.NAND:
                                int temporaryResult = outputStates[i] * outputStates[i + 1];

                                if (temporaryResult == 1)
                                    newOutputStates[counter] = 0;
                                else
                                    newOutputStates[counter] = 1;
                                break;

                            case GateType.OR:
                                newOutputStates[counter] = outputStates[i] >= outputStates[i + 1] ? outputStates[i] : outputStates[i + 1];
                                break;

                            case GateType.NOR:
                                newOutputStates[counter] = outputStates[i] >= outputStates[i + 1] ? outputStates[i + 1] : outputStates[i];
                                break;

                            default:
                                return -1;
                        }

                        counter++;
                    }

                    if (inputNumber % 2 > 0)
                    {
                        newOutputStates[counter + 1] = outputStates[counter + 1];
                    }

                    if ((inputNumber / 2 + inputNumber % 2) > 1)
                    {
                        this.recurenceCalculation((inputNumber / 2 + inputNumber % 2), newOutputStates, gateType);
                    }
                    else
                    {
                        result = newOutputStates[0];
                        return result;
                    }

                    return -1;
                }
                else
                {
                    if (outputStates[0] == -1)
                        return -1;

                    return outputStates[0] == 1 ? 0 : 1;
                }

            }
            public  DataModels.OutpueEnum CalculateOutput(int myId)
            {
                DataContainer.ElementContainer selfElement = new DataContainer.ElementContainer();

                bool existenceFlag = false;

                foreach(DataContainer.ElementContainer element in DataContainer.Data.Instance.elementList)
                {
                    if (element.id == myId)
                    {
                        selfElement = element;
                        existenceFlag = true;
                        break;
                    }
                    else
                    {
                        existenceFlag = false;
                    }
                }

                if (!existenceFlag)
                {
                    return DataModels.OutpueEnum.unknown;
                }

                DataContainer.ElementContainer[] desiredElements = new DataContainer.ElementContainer[selfElement.inputNumber];

                if (selfElement.elementType == ElementType.gateType)
                {
                    for (int i = 0; i < selfElement.inputNumber; i++)
                    {

                        int id = selfElement.linkedElem[i];

                        foreach (DataContainer.ElementContainer element in DataContainer.Data.Instance.elementList)
                        {
                            if (element.id == id)
                            {
                                desiredElements[i] = element;
                                break;
                            }
                        }
                    }

                    int[] outputStates = new int[selfElement.inputNumber];

                    for (int i = 0; i < selfElement.inputNumber; i++)
                    {
                        outputStates[i] = (int)desiredElements[i].state;

                        if (outputStates[i] == -1)
                        {
                            return OutpueEnum.unknown;
                        }
                    }

                    int result = this.recurenceCalculation(selfElement.inputNumber, outputStates, selfElement.gateType);

                    DataModels.OutpueEnum state = OutpueEnum.unknown;

                    switch (result)
                    {
                        case 1:
                            state = OutpueEnum.high;
                            break;
                        case 0:
                            state = OutpueEnum.low;
                            break;
                        case -1:
                            state = OutpueEnum.unknown;
                            break;
                    }

                    return state;
                }
                else
                {
                    return selfElement.state;
                }
            }

            public bool LinkElem(int targetId, int myId)
            {
                return true;
            }

            public bool DelElem(int myId)
            {
                return true;
            }
        }

        public class AND : Element
        {


        }
        public class NOT : Element
        {

        }

        public class OR : Element
        {

        }


        public class NAND : Element
        {

        }


        public class NOR : Element
        {


        }

        public class OutputPoint : Element
        {


        }

        public class Junction : Element
        {


        }

        public class InBus : Element
        {


        }


    }

    namespace Utilities
    {

    }

    namespace ElementInformator
    {


    }
}

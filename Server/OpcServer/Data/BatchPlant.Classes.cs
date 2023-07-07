/* ========================================================================
 * Copyright (c) 2005-2016 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

using Opc.Ua;
using System.Collections.Generic;

namespace OpcServer.Data
{
    #region Method Identifiers
    /// <summary>
    /// A class that declares constants for all Methods in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCode("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Methods
    {
        /// <summary>
        /// The identifier for the BatchPlantType_StartProcess Method.
        /// </summary>
        public const uint BatchPlantType_StartProcess = 15136;

        /// <summary>
        /// The identifier for the BatchPlantType_StopProcess Method.
        /// </summary>
        public const uint BatchPlantType_StopProcess = 15137;

        /// <summary>
        /// The identifier for the BatchPlant1_StartProcess Method.
        /// </summary>
        public const uint BatchPlant1_StartProcess = 15176;

        /// <summary>
        /// The identifier for the BatchPlant1_StopProcess Method.
        /// </summary>
        public const uint BatchPlant1_StopProcess = 15177;
    }
    #endregion

    #region Object Identifiers
    /// <summary>
    /// A class that declares constants for all Objects in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCode("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Objects
    {
        /// <summary>
        /// The identifier for the MixerType_LoadcellTransmitter Object.
        /// </summary>
        public const uint MixerType_LoadcellTransmitter = 15064;

        /// <summary>
        /// The identifier for the MixerType_MixerMotor Object.
        /// </summary>
        public const uint MixerType_MixerMotor = 15078;

        /// <summary>
        /// The identifier for the MixerType_MixerDischargeValve Object.
        /// </summary>
        public const uint MixerType_MixerDischargeValve = 15085;

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer Object.
        /// </summary>
        public const uint BatchPlantType_Mixer = 15099;

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_LoadcellTransmitter Object.
        /// </summary>
        public const uint BatchPlantType_Mixer_LoadcellTransmitter = 15100;

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_MixerMotor Object.
        /// </summary>
        public const uint BatchPlantType_Mixer_MixerMotor = 15114;

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_MixerDischargeValve Object.
        /// </summary>
        public const uint BatchPlantType_Mixer_MixerDischargeValve = 15121;

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_DischargeValve Object.
        /// </summary>
        public const uint BatchPlantType_Mixer_DischargeValve = 15134;

        /// <summary>
        /// The identifier for the BatchPlant1 Object.
        /// </summary>
        public const uint BatchPlant1 = 15138;

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer Object.
        /// </summary>
        public const uint BatchPlant1_Mixer = 15139;

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_LoadcellTransmitter Object.
        /// </summary>
        public const uint BatchPlant1_Mixer_LoadcellTransmitter = 15140;

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_MixerMotor Object.
        /// </summary>
        public const uint BatchPlant1_Mixer_MixerMotor = 15154;

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_MixerDischargeValve Object.
        /// </summary>
        public const uint BatchPlant1_Mixer_MixerDischargeValve = 15161;

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_DischargeValve Object.
        /// </summary>
        public const uint BatchPlant1_Mixer_DischargeValve = 15174;
    }
    #endregion

    #region ObjectType Identifiers
    /// <summary>
    /// A class that declares constants for all ObjectTypes in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCode("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectTypes
    {
        /// <summary>
        /// The identifier for the GenericMotorType ObjectType.
        /// </summary>
        public const uint GenericMotorType = 15001;

        /// <summary>
        /// The identifier for the GenericSensorType ObjectType.
        /// </summary>
        public const uint GenericSensorType = 15008;

        /// <summary>
        /// The identifier for the GenericActuatorType ObjectType.
        /// </summary>
        public const uint GenericActuatorType = 15016;

        /// <summary>
        /// The identifier for the LoadcellTransmitterType ObjectType.
        /// </summary>
        public const uint LoadcellTransmitterType = 15029;

        /// <summary>
        /// The identifier for the ValveType ObjectType.
        /// </summary>
        public const uint ValveType = 15043;

        /// <summary>
        /// The identifier for the MixerMotorType ObjectType.
        /// </summary>
        public const uint MixerMotorType = 15056;

        /// <summary>
        /// The identifier for the MixerType ObjectType.
        /// </summary>
        public const uint MixerType = 15063;

        /// <summary>
        /// The identifier for the BatchPlantType ObjectType.
        /// </summary>
        public const uint BatchPlantType = 15098;
    }
    #endregion

    #region Variable Identifiers
    /// <summary>
    /// A class that declares constants for all Variables in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCode("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Variables
    {
        /// <summary>
        /// The identifier for the GenericMotorType_Speed Variable.
        /// </summary>
        public const uint GenericMotorType_Speed = 15002;

        /// <summary>
        /// The identifier for the GenericMotorType_Speed_EURange Variable.
        /// </summary>
        public const uint GenericMotorType_Speed_EURange = 15006;

        /// <summary>
        /// The identifier for the GenericSensorType_Output Variable.
        /// </summary>
        public const uint GenericSensorType_Output = 15009;

        /// <summary>
        /// The identifier for the GenericSensorType_Output_EURange Variable.
        /// </summary>
        public const uint GenericSensorType_Output_EURange = 15013;

        /// <summary>
        /// The identifier for the GenericSensorType_Units Variable.
        /// </summary>
        public const uint GenericSensorType_Units = 15015;

        /// <summary>
        /// The identifier for the GenericActuatorType_Input Variable.
        /// </summary>
        public const uint GenericActuatorType_Input = 15017;

        /// <summary>
        /// The identifier for the GenericActuatorType_Input_EURange Variable.
        /// </summary>
        public const uint GenericActuatorType_Input_EURange = 15021;

        /// <summary>
        /// The identifier for the GenericActuatorType_Output Variable.
        /// </summary>
        public const uint GenericActuatorType_Output = 15023;

        /// <summary>
        /// The identifier for the GenericActuatorType_Output_EURange Variable.
        /// </summary>
        public const uint GenericActuatorType_Output_EURange = 15027;

        /// <summary>
        /// The identifier for the LoadcellTransmitterType_Output_EURange Variable.
        /// </summary>
        public const uint LoadcellTransmitterType_Output_EURange = 15034;

        /// <summary>
        /// The identifier for the LoadcellTransmitterType_ExcitationVoltage Variable.
        /// </summary>
        public const uint LoadcellTransmitterType_ExcitationVoltage = 15037;

        /// <summary>
        /// The identifier for the LoadcellTransmitterType_ExcitationVoltage_EURange Variable.
        /// </summary>
        public const uint LoadcellTransmitterType_ExcitationVoltage_EURange = 15041;

        /// <summary>
        /// The identifier for the ValveType_Input_EURange Variable.
        /// </summary>
        public const uint ValveType_Input_EURange = 15048;

        /// <summary>
        /// The identifier for the ValveType_Output_EURange Variable.
        /// </summary>
        public const uint ValveType_Output_EURange = 15054;

        /// <summary>
        /// The identifier for the MixerMotorType_Speed_EURange Variable.
        /// </summary>
        public const uint MixerMotorType_Speed_EURange = 15061;

        /// <summary>
        /// The identifier for the MixerType_LoadcellTransmitter_Output Variable.
        /// </summary>
        public const uint MixerType_LoadcellTransmitter_Output = 15065;

        /// <summary>
        /// The identifier for the MixerType_LoadcellTransmitter_Output_EURange Variable.
        /// </summary>
        public const uint MixerType_LoadcellTransmitter_Output_EURange = 15069;

        /// <summary>
        /// The identifier for the MixerType_LoadcellTransmitter_Units Variable.
        /// </summary>
        public const uint MixerType_LoadcellTransmitter_Units = 15071;

        /// <summary>
        /// The identifier for the MixerType_LoadcellTransmitter_ExcitationVoltage Variable.
        /// </summary>
        public const uint MixerType_LoadcellTransmitter_ExcitationVoltage = 15072;

        /// <summary>
        /// The identifier for the MixerType_LoadcellTransmitter_ExcitationVoltage_EURange Variable.
        /// </summary>
        public const uint MixerType_LoadcellTransmitter_ExcitationVoltage_EURange = 15076;

        /// <summary>
        /// The identifier for the MixerType_MixerMotor_Speed Variable.
        /// </summary>
        public const uint MixerType_MixerMotor_Speed = 15079;

        /// <summary>
        /// The identifier for the MixerType_MixerMotor_Speed_EURange Variable.
        /// </summary>
        public const uint MixerType_MixerMotor_Speed_EURange = 15083;

        /// <summary>
        /// The identifier for the MixerType_MixerDischargeValve_Input Variable.
        /// </summary>
        public const uint MixerType_MixerDischargeValve_Input = 15086;

        /// <summary>
        /// The identifier for the MixerType_MixerDischargeValve_Input_EURange Variable.
        /// </summary>
        public const uint MixerType_MixerDischargeValve_Input_EURange = 15090;

        /// <summary>
        /// The identifier for the MixerType_MixerDischargeValve_Output Variable.
        /// </summary>
        public const uint MixerType_MixerDischargeValve_Output = 15092;

        /// <summary>
        /// The identifier for the MixerType_MixerDischargeValve_Output_EURange Variable.
        /// </summary>
        public const uint MixerType_MixerDischargeValve_Output_EURange = 15096;

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_LoadcellTransmitter_Output Variable.
        /// </summary>
        public const uint BatchPlantType_Mixer_LoadcellTransmitter_Output = 15101;

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_LoadcellTransmitter_Output_EURange Variable.
        /// </summary>
        public const uint BatchPlantType_Mixer_LoadcellTransmitter_Output_EURange = 15105;

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_LoadcellTransmitter_Units Variable.
        /// </summary>
        public const uint BatchPlantType_Mixer_LoadcellTransmitter_Units = 15107;

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_LoadcellTransmitter_ExcitationVoltage Variable.
        /// </summary>
        public const uint BatchPlantType_Mixer_LoadcellTransmitter_ExcitationVoltage = 15108;

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_LoadcellTransmitter_ExcitationVoltage_EURange Variable.
        /// </summary>
        public const uint BatchPlantType_Mixer_LoadcellTransmitter_ExcitationVoltage_EURange = 15112;

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_MixerMotor_Speed Variable.
        /// </summary>
        public const uint BatchPlantType_Mixer_MixerMotor_Speed = 15115;

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_MixerMotor_Speed_EURange Variable.
        /// </summary>
        public const uint BatchPlantType_Mixer_MixerMotor_Speed_EURange = 15119;

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_MixerDischargeValve_Input Variable.
        /// </summary>
        public const uint BatchPlantType_Mixer_MixerDischargeValve_Input = 15122;

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_MixerDischargeValve_Input_EURange Variable.
        /// </summary>
        public const uint BatchPlantType_Mixer_MixerDischargeValve_Input_EURange = 15126;

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_MixerDischargeValve_Output Variable.
        /// </summary>
        public const uint BatchPlantType_Mixer_MixerDischargeValve_Output = 15128;

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_MixerDischargeValve_Output_EURange Variable.
        /// </summary>
        public const uint BatchPlantType_Mixer_MixerDischargeValve_Output_EURange = 15132;

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_DischargeValve_Input Variable.
        /// </summary>
        public const uint BatchPlantType_Mixer_DischargeValve_Input = 15135;

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_LoadcellTransmitter_Output Variable.
        /// </summary>
        public const uint BatchPlant1_Mixer_LoadcellTransmitter_Output = 15141;

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_LoadcellTransmitter_Output_EURange Variable.
        /// </summary>
        public const uint BatchPlant1_Mixer_LoadcellTransmitter_Output_EURange = 15145;

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_LoadcellTransmitter_Units Variable.
        /// </summary>
        public const uint BatchPlant1_Mixer_LoadcellTransmitter_Units = 15147;

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_LoadcellTransmitter_ExcitationVoltage Variable.
        /// </summary>
        public const uint BatchPlant1_Mixer_LoadcellTransmitter_ExcitationVoltage = 15148;

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_LoadcellTransmitter_ExcitationVoltage_EURange Variable.
        /// </summary>
        public const uint BatchPlant1_Mixer_LoadcellTransmitter_ExcitationVoltage_EURange = 15152;

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_MixerMotor_Speed Variable.
        /// </summary>
        public const uint BatchPlant1_Mixer_MixerMotor_Speed = 15155;

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_MixerMotor_Speed_EURange Variable.
        /// </summary>
        public const uint BatchPlant1_Mixer_MixerMotor_Speed_EURange = 15159;

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_MixerDischargeValve_Input Variable.
        /// </summary>
        public const uint BatchPlant1_Mixer_MixerDischargeValve_Input = 15162;

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_MixerDischargeValve_Input_EURange Variable.
        /// </summary>
        public const uint BatchPlant1_Mixer_MixerDischargeValve_Input_EURange = 15166;

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_MixerDischargeValve_Output Variable.
        /// </summary>
        public const uint BatchPlant1_Mixer_MixerDischargeValve_Output = 15168;

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_MixerDischargeValve_Output_EURange Variable.
        /// </summary>
        public const uint BatchPlant1_Mixer_MixerDischargeValve_Output_EURange = 15172;

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_DischargeValve_Input Variable.
        /// </summary>
        public const uint BatchPlant1_Mixer_DischargeValve_Input = 15175;
    }
    #endregion

    #region Method Node Identifiers
    /// <summary>
    /// A class that declares constants for all Methods in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCode("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class MethodIds
    {
        /// <summary>
        /// The identifier for the BatchPlantType_StartProcess Method.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlantType_StartProcess = new ExpandedNodeId(Methods.BatchPlantType_StartProcess, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlantType_StopProcess Method.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlantType_StopProcess = new ExpandedNodeId(Methods.BatchPlantType_StopProcess, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlant1_StartProcess Method.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlant1_StartProcess = new ExpandedNodeId(Methods.BatchPlant1_StartProcess, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlant1_StopProcess Method.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlant1_StopProcess = new ExpandedNodeId(Methods.BatchPlant1_StopProcess, Namespaces.BatchPlant);
    }
    #endregion

    #region Object Node Identifiers
    /// <summary>
    /// A class that declares constants for all Objects in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCode("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectIds
    {
        /// <summary>
        /// The identifier for the MixerType_LoadcellTransmitter Object.
        /// </summary>
        public static readonly ExpandedNodeId MixerType_LoadcellTransmitter = new ExpandedNodeId(Objects.MixerType_LoadcellTransmitter, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the MixerType_MixerMotor Object.
        /// </summary>
        public static readonly ExpandedNodeId MixerType_MixerMotor = new ExpandedNodeId(Objects.MixerType_MixerMotor, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the MixerType_MixerDischargeValve Object.
        /// </summary>
        public static readonly ExpandedNodeId MixerType_MixerDischargeValve = new ExpandedNodeId(Objects.MixerType_MixerDischargeValve, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer Object.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlantType_Mixer = new ExpandedNodeId(Objects.BatchPlantType_Mixer, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_LoadcellTransmitter Object.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlantType_Mixer_LoadcellTransmitter = new ExpandedNodeId(Objects.BatchPlantType_Mixer_LoadcellTransmitter, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_MixerMotor Object.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlantType_Mixer_MixerMotor = new ExpandedNodeId(Objects.BatchPlantType_Mixer_MixerMotor, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_MixerDischargeValve Object.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlantType_Mixer_MixerDischargeValve = new ExpandedNodeId(Objects.BatchPlantType_Mixer_MixerDischargeValve, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_DischargeValve Object.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlantType_Mixer_DischargeValve = new ExpandedNodeId(Objects.BatchPlantType_Mixer_DischargeValve, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlant1 Object.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlant1 = new ExpandedNodeId(Objects.BatchPlant1, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer Object.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlant1_Mixer = new ExpandedNodeId(Objects.BatchPlant1_Mixer, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_LoadcellTransmitter Object.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlant1_Mixer_LoadcellTransmitter = new ExpandedNodeId(Objects.BatchPlant1_Mixer_LoadcellTransmitter, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_MixerMotor Object.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlant1_Mixer_MixerMotor = new ExpandedNodeId(Objects.BatchPlant1_Mixer_MixerMotor, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_MixerDischargeValve Object.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlant1_Mixer_MixerDischargeValve = new ExpandedNodeId(Objects.BatchPlant1_Mixer_MixerDischargeValve, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_DischargeValve Object.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlant1_Mixer_DischargeValve = new ExpandedNodeId(Objects.BatchPlant1_Mixer_DischargeValve, Namespaces.BatchPlant);
    }
    #endregion

    #region ObjectType Node Identifiers
    /// <summary>
    /// A class that declares constants for all ObjectTypes in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCode("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectTypeIds
    {
        /// <summary>
        /// The identifier for the GenericMotorType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId GenericMotorType = new ExpandedNodeId(ObjectTypes.GenericMotorType, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the GenericSensorType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId GenericSensorType = new ExpandedNodeId(ObjectTypes.GenericSensorType, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the GenericActuatorType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId GenericActuatorType = new ExpandedNodeId(ObjectTypes.GenericActuatorType, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the LoadcellTransmitterType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId LoadcellTransmitterType = new ExpandedNodeId(ObjectTypes.LoadcellTransmitterType, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the ValveType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId ValveType = new ExpandedNodeId(ObjectTypes.ValveType, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the MixerMotorType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId MixerMotorType = new ExpandedNodeId(ObjectTypes.MixerMotorType, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the MixerType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId MixerType = new ExpandedNodeId(ObjectTypes.MixerType, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlantType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlantType = new ExpandedNodeId(ObjectTypes.BatchPlantType, Namespaces.BatchPlant);
    }
    #endregion

    #region Variable Node Identifiers
    /// <summary>
    /// A class that declares constants for all Variables in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCode("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class VariableIds
    {
        /// <summary>
        /// The identifier for the GenericMotorType_Speed Variable.
        /// </summary>
        public static readonly ExpandedNodeId GenericMotorType_Speed = new ExpandedNodeId(Variables.GenericMotorType_Speed, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the GenericMotorType_Speed_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId GenericMotorType_Speed_EURange = new ExpandedNodeId(Variables.GenericMotorType_Speed_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the GenericSensorType_Output Variable.
        /// </summary>
        public static readonly ExpandedNodeId GenericSensorType_Output = new ExpandedNodeId(Variables.GenericSensorType_Output, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the GenericSensorType_Output_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId GenericSensorType_Output_EURange = new ExpandedNodeId(Variables.GenericSensorType_Output_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the GenericSensorType_Units Variable.
        /// </summary>
        public static readonly ExpandedNodeId GenericSensorType_Units = new ExpandedNodeId(Variables.GenericSensorType_Units, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the GenericActuatorType_Input Variable.
        /// </summary>
        public static readonly ExpandedNodeId GenericActuatorType_Input = new ExpandedNodeId(Variables.GenericActuatorType_Input, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the GenericActuatorType_Input_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId GenericActuatorType_Input_EURange = new ExpandedNodeId(Variables.GenericActuatorType_Input_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the GenericActuatorType_Output Variable.
        /// </summary>
        public static readonly ExpandedNodeId GenericActuatorType_Output = new ExpandedNodeId(Variables.GenericActuatorType_Output, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the GenericActuatorType_Output_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId GenericActuatorType_Output_EURange = new ExpandedNodeId(Variables.GenericActuatorType_Output_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the LoadcellTransmitterType_Output_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId LoadcellTransmitterType_Output_EURange = new ExpandedNodeId(Variables.LoadcellTransmitterType_Output_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the LoadcellTransmitterType_ExcitationVoltage Variable.
        /// </summary>
        public static readonly ExpandedNodeId LoadcellTransmitterType_ExcitationVoltage = new ExpandedNodeId(Variables.LoadcellTransmitterType_ExcitationVoltage, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the LoadcellTransmitterType_ExcitationVoltage_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId LoadcellTransmitterType_ExcitationVoltage_EURange = new ExpandedNodeId(Variables.LoadcellTransmitterType_ExcitationVoltage_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the ValveType_Input_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId ValveType_Input_EURange = new ExpandedNodeId(Variables.ValveType_Input_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the ValveType_Output_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId ValveType_Output_EURange = new ExpandedNodeId(Variables.ValveType_Output_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the MixerMotorType_Speed_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId MixerMotorType_Speed_EURange = new ExpandedNodeId(Variables.MixerMotorType_Speed_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the MixerType_LoadcellTransmitter_Output Variable.
        /// </summary>
        public static readonly ExpandedNodeId MixerType_LoadcellTransmitter_Output = new ExpandedNodeId(Variables.MixerType_LoadcellTransmitter_Output, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the MixerType_LoadcellTransmitter_Output_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId MixerType_LoadcellTransmitter_Output_EURange = new ExpandedNodeId(Variables.MixerType_LoadcellTransmitter_Output_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the MixerType_LoadcellTransmitter_Units Variable.
        /// </summary>
        public static readonly ExpandedNodeId MixerType_LoadcellTransmitter_Units = new ExpandedNodeId(Variables.MixerType_LoadcellTransmitter_Units, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the MixerType_LoadcellTransmitter_ExcitationVoltage Variable.
        /// </summary>
        public static readonly ExpandedNodeId MixerType_LoadcellTransmitter_ExcitationVoltage = new ExpandedNodeId(Variables.MixerType_LoadcellTransmitter_ExcitationVoltage, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the MixerType_LoadcellTransmitter_ExcitationVoltage_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId MixerType_LoadcellTransmitter_ExcitationVoltage_EURange = new ExpandedNodeId(Variables.MixerType_LoadcellTransmitter_ExcitationVoltage_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the MixerType_MixerMotor_Speed Variable.
        /// </summary>
        public static readonly ExpandedNodeId MixerType_MixerMotor_Speed = new ExpandedNodeId(Variables.MixerType_MixerMotor_Speed, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the MixerType_MixerMotor_Speed_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId MixerType_MixerMotor_Speed_EURange = new ExpandedNodeId(Variables.MixerType_MixerMotor_Speed_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the MixerType_MixerDischargeValve_Input Variable.
        /// </summary>
        public static readonly ExpandedNodeId MixerType_MixerDischargeValve_Input = new ExpandedNodeId(Variables.MixerType_MixerDischargeValve_Input, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the MixerType_MixerDischargeValve_Input_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId MixerType_MixerDischargeValve_Input_EURange = new ExpandedNodeId(Variables.MixerType_MixerDischargeValve_Input_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the MixerType_MixerDischargeValve_Output Variable.
        /// </summary>
        public static readonly ExpandedNodeId MixerType_MixerDischargeValve_Output = new ExpandedNodeId(Variables.MixerType_MixerDischargeValve_Output, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the MixerType_MixerDischargeValve_Output_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId MixerType_MixerDischargeValve_Output_EURange = new ExpandedNodeId(Variables.MixerType_MixerDischargeValve_Output_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_LoadcellTransmitter_Output Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlantType_Mixer_LoadcellTransmitter_Output = new ExpandedNodeId(Variables.BatchPlantType_Mixer_LoadcellTransmitter_Output, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_LoadcellTransmitter_Output_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlantType_Mixer_LoadcellTransmitter_Output_EURange = new ExpandedNodeId(Variables.BatchPlantType_Mixer_LoadcellTransmitter_Output_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_LoadcellTransmitter_Units Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlantType_Mixer_LoadcellTransmitter_Units = new ExpandedNodeId(Variables.BatchPlantType_Mixer_LoadcellTransmitter_Units, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_LoadcellTransmitter_ExcitationVoltage Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlantType_Mixer_LoadcellTransmitter_ExcitationVoltage = new ExpandedNodeId(Variables.BatchPlantType_Mixer_LoadcellTransmitter_ExcitationVoltage, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_LoadcellTransmitter_ExcitationVoltage_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlantType_Mixer_LoadcellTransmitter_ExcitationVoltage_EURange = new ExpandedNodeId(Variables.BatchPlantType_Mixer_LoadcellTransmitter_ExcitationVoltage_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_MixerMotor_Speed Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlantType_Mixer_MixerMotor_Speed = new ExpandedNodeId(Variables.BatchPlantType_Mixer_MixerMotor_Speed, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_MixerMotor_Speed_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlantType_Mixer_MixerMotor_Speed_EURange = new ExpandedNodeId(Variables.BatchPlantType_Mixer_MixerMotor_Speed_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_MixerDischargeValve_Input Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlantType_Mixer_MixerDischargeValve_Input = new ExpandedNodeId(Variables.BatchPlantType_Mixer_MixerDischargeValve_Input, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_MixerDischargeValve_Input_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlantType_Mixer_MixerDischargeValve_Input_EURange = new ExpandedNodeId(Variables.BatchPlantType_Mixer_MixerDischargeValve_Input_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_MixerDischargeValve_Output Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlantType_Mixer_MixerDischargeValve_Output = new ExpandedNodeId(Variables.BatchPlantType_Mixer_MixerDischargeValve_Output, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_MixerDischargeValve_Output_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlantType_Mixer_MixerDischargeValve_Output_EURange = new ExpandedNodeId(Variables.BatchPlantType_Mixer_MixerDischargeValve_Output_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlantType_Mixer_DischargeValve_Input Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlantType_Mixer_DischargeValve_Input = new ExpandedNodeId(Variables.BatchPlantType_Mixer_DischargeValve_Input, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_LoadcellTransmitter_Output Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlant1_Mixer_LoadcellTransmitter_Output = new ExpandedNodeId(Variables.BatchPlant1_Mixer_LoadcellTransmitter_Output, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_LoadcellTransmitter_Output_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlant1_Mixer_LoadcellTransmitter_Output_EURange = new ExpandedNodeId(Variables.BatchPlant1_Mixer_LoadcellTransmitter_Output_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_LoadcellTransmitter_Units Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlant1_Mixer_LoadcellTransmitter_Units = new ExpandedNodeId(Variables.BatchPlant1_Mixer_LoadcellTransmitter_Units, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_LoadcellTransmitter_ExcitationVoltage Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlant1_Mixer_LoadcellTransmitter_ExcitationVoltage = new ExpandedNodeId(Variables.BatchPlant1_Mixer_LoadcellTransmitter_ExcitationVoltage, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_LoadcellTransmitter_ExcitationVoltage_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlant1_Mixer_LoadcellTransmitter_ExcitationVoltage_EURange = new ExpandedNodeId(Variables.BatchPlant1_Mixer_LoadcellTransmitter_ExcitationVoltage_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_MixerMotor_Speed Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlant1_Mixer_MixerMotor_Speed = new ExpandedNodeId(Variables.BatchPlant1_Mixer_MixerMotor_Speed, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_MixerMotor_Speed_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlant1_Mixer_MixerMotor_Speed_EURange = new ExpandedNodeId(Variables.BatchPlant1_Mixer_MixerMotor_Speed_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_MixerDischargeValve_Input Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlant1_Mixer_MixerDischargeValve_Input = new ExpandedNodeId(Variables.BatchPlant1_Mixer_MixerDischargeValve_Input, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_MixerDischargeValve_Input_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlant1_Mixer_MixerDischargeValve_Input_EURange = new ExpandedNodeId(Variables.BatchPlant1_Mixer_MixerDischargeValve_Input_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_MixerDischargeValve_Output Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlant1_Mixer_MixerDischargeValve_Output = new ExpandedNodeId(Variables.BatchPlant1_Mixer_MixerDischargeValve_Output, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_MixerDischargeValve_Output_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlant1_Mixer_MixerDischargeValve_Output_EURange = new ExpandedNodeId(Variables.BatchPlant1_Mixer_MixerDischargeValve_Output_EURange, Namespaces.BatchPlant);

        /// <summary>
        /// The identifier for the BatchPlant1_Mixer_DischargeValve_Input Variable.
        /// </summary>
        public static readonly ExpandedNodeId BatchPlant1_Mixer_DischargeValve_Input = new ExpandedNodeId(Variables.BatchPlant1_Mixer_DischargeValve_Input, Namespaces.BatchPlant);
    }
    #endregion

    #region BrowseName Declarations
    /// <summary>
    /// Declares all of the BrowseNames used in the Model Design.
    /// </summary>
    public static partial class BrowseNames
    {
        /// <summary>
        /// The BrowseName for the BatchPlant1 component.
        /// </summary>
        public const string BatchPlant1 = "Batch Plant #1";

        /// <summary>
        /// The BrowseName for the BatchPlantType component.
        /// </summary>
        public const string BatchPlantType = "BatchPlantType";

        /// <summary>
        /// The BrowseName for the ExcitationVoltage component.
        /// </summary>
        public const string ExcitationVoltage = "ExcitationVoltage";

        /// <summary>
        /// The BrowseName for the GenericActuatorType component.
        /// </summary>
        public const string GenericActuatorType = "GenericActuatorType";

        /// <summary>
        /// The BrowseName for the GenericMotorType component.
        /// </summary>
        public const string GenericMotorType = "GenericMotorType";

        /// <summary>
        /// The BrowseName for the GenericSensorType component.
        /// </summary>
        public const string GenericSensorType = "GenericSensorType";

        /// <summary>
        /// The BrowseName for the Input component.
        /// </summary>
        public const string Input = "Input";

        /// <summary>
        /// The BrowseName for the LoadcellTransmitter component.
        /// </summary>
        public const string LoadcellTransmitter = "LT001";

        /// <summary>
        /// The BrowseName for the LoadcellTransmitterType component.
        /// </summary>
        public const string LoadcellTransmitterType = "LoadcellTransmitterType";

        /// <summary>
        /// The BrowseName for the Mixer component.
        /// </summary>
        public const string Mixer = "MixerX001";

        /// <summary>
        /// The BrowseName for the MixerDischargeValve component.
        /// </summary>
        public const string MixerDischargeValve = "Valve004";

        /// <summary>
        /// The BrowseName for the MixerMotor component.
        /// </summary>
        public const string MixerMotor = "Motor01";

        /// <summary>
        /// The BrowseName for the MixerMotorType component.
        /// </summary>
        public const string MixerMotorType = "MixerMotorType";

        /// <summary>
        /// The BrowseName for the MixerType component.
        /// </summary>
        public const string MixerType = "MixerType";

        /// <summary>
        /// The BrowseName for the Output component.
        /// </summary>
        public const string Output = "Output";

        /// <summary>
        /// The BrowseName for the Speed component.
        /// </summary>
        public const string Speed = "Speed";

        /// <summary>
        /// The BrowseName for the StartProcess component.
        /// </summary>
        public const string StartProcess = "StartProcess";

        /// <summary>
        /// The BrowseName for the StopProcess component.
        /// </summary>
        public const string StopProcess = "StopProcess";

        /// <summary>
        /// The BrowseName for the Units component.
        /// </summary>
        public const string Units = "Units";

        /// <summary>
        /// The BrowseName for the ValveType component.
        /// </summary>
        public const string ValveType = "ValveType";
    }
    #endregion

    #region Namespace Declarations
    /// <summary>
    /// Defines constants for all namespaces referenced by the model design.
    /// </summary>
    public static partial class Namespaces
    {
        /// <summary>
        /// The URI for the OpcUa namespace (.NET code namespace is 'Opc.Ua').
        /// </summary>
        public const string OpcUa = "http://opcfoundation.org/UA/";

        /// <summary>
        /// The URI for the OpcUaXsd namespace (.NET code namespace is 'Opc.Ua').
        /// </summary>
        public const string OpcUaXsd = "http://opcfoundation.org/UA/2008/02/Types.xsd";

        /// <summary>
        /// The URI for the BatchPlant namespace (.NET code namespace is 'BatchPlant').
        /// </summary>
        public const string BatchPlant = "http://opcfoundation.org/BatchPlant";
    }
    #endregion

    #region GenericMotorState Class
#if (!OPCUA_EXCLUDE_GenericMotorState)
    /// <summary>
    /// Stores an instance of the GenericMotorType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCode("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class GenericMotorState : BaseObjectState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public GenericMotorState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(ObjectTypes.GenericMotorType, Namespaces.BatchPlant, namespaceUris);
        }

#if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the instance with a node.
        /// </summary>
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AgAAACAAAAAKCQkJaHR0cDovL29wY2ZvdW5kYXRpb24ub3JnL1VBLyMAAABodHRwOi8vb3BjZm91bmRh" +
           "dGlvbi5vcmcvQmF0Y2hQbGFudP////8EYIAAAQAAAAIAGAAAAEdlbmVyaWNNb3RvclR5cGVJbnN0YW5j" +
           "ZQECmToBApk6/////wEAAAAVYIkKAgAAAAIABQAAAFNwZWVkAQKaOgAvAQBACZo6AAAAC/////8DA///" +
           "//8BAAAAFWCJCgIAAAAAAAcAAABFVVJhbmdlAQKeOgAuAESeOgAAAQB0A/////8BAf////8AAAAA";
        #endregion
#endif
        #endregion

        #region Public Properties
        /// <summary>
        /// A description for the Speed Variable.
        /// </summary>
        public AnalogItemState<double> Speed
        {
            get
            {
                return m_speed;
            }

            set
            {
                if (!object.ReferenceEquals(m_speed, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_speed = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Populates a list with the children that belong to the node.
        /// </summary>
        /// <param name="context">The context for the system being accessed.</param>
        /// <param name="children">The list of children to populate.</param>
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_speed != null)
            {
                children.Add(m_speed);
            }

            base.GetChildren(context, children);
        }

        /// <summary>
        /// Finds the child with the specified browse name.
        /// </summary>
        protected override BaseInstanceState FindChild(
            ISystemContext context,
            QualifiedName browseName,
            bool createOrReplace,
            BaseInstanceState replacement)
        {
            if (QualifiedName.IsNull(browseName))
            {
                return null;
            }

            BaseInstanceState instance = null;

            switch (browseName.Name)
            {
                case BrowseNames.Speed:
                    {
                        if (createOrReplace)
                        {
                            if (Speed == null)
                            {
                                if (replacement == null)
                                {
                                    Speed = new AnalogItemState<double>(this);
                                }
                                else
                                {
                                    Speed = (AnalogItemState<double>)replacement;
                                }
                            }
                        }

                        instance = Speed;
                        break;
                    }
            }

            if (instance != null)
            {
                return instance;
            }

            return base.FindChild(context, browseName, createOrReplace, replacement);
        }
        #endregion

        #region Private Fields
        private AnalogItemState<double> m_speed;
        #endregion
    }
#endif
    #endregion

    #region GenericSensorState Class
#if (!OPCUA_EXCLUDE_GenericSensorState)
    /// <summary>
    /// Stores an instance of the GenericSensorType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCode("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class GenericSensorState : BaseObjectState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public GenericSensorState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(ObjectTypes.GenericSensorType, Namespaces.BatchPlant, namespaceUris);
        }

#if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the instance with a node.
        /// </summary>
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AgAAACAAAAAKCQkJaHR0cDovL29wY2ZvdW5kYXRpb24ub3JnL1VBLyMAAABodHRwOi8vb3BjZm91bmRh" +
           "dGlvbi5vcmcvQmF0Y2hQbGFudP////8EYIAAAQAAAAIAGQAAAEdlbmVyaWNTZW5zb3JUeXBlSW5zdGFu" +
           "Y2UBAqA6AQKgOv////8CAAAAFWCJCgIAAAACAAYAAABPdXRwdXQBAqE6AC8BAEAJoToAAAAL/////wEB" +
           "/////wEAAAAVYIkKAgAAAAAABwAAAEVVUmFuZ2UBAqU6AC4ARKU6AAABAHQD/////wEB/////wAAAAAV" +
           "YIkKAgAAAAIABQAAAFVuaXRzAQKnOgAuAESnOgAAAAz/////AwP/////AAAAAA==";
        #endregion
#endif
        #endregion

        #region Public Properties
        /// <summary>
        /// A description for the Output Variable.
        /// </summary>
        public AnalogItemState<double> Output
        {
            get
            {
                return m_output;
            }

            set
            {
                if (!object.ReferenceEquals(m_output, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_output = value;
            }
        }

        /// <summary>
        /// A description for the Units Property.
        /// </summary>
        public PropertyState<string> Units
        {
            get
            {
                return m_units;
            }

            set
            {
                if (!object.ReferenceEquals(m_units, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_units = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Populates a list with the children that belong to the node.
        /// </summary>
        /// <param name="context">The context for the system being accessed.</param>
        /// <param name="children">The list of children to populate.</param>
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_output != null)
            {
                children.Add(m_output);
            }

            if (m_units != null)
            {
                children.Add(m_units);
            }

            base.GetChildren(context, children);
        }

        /// <summary>
        /// Finds the child with the specified browse name.
        /// </summary>
        protected override BaseInstanceState FindChild(
            ISystemContext context,
            QualifiedName browseName,
            bool createOrReplace,
            BaseInstanceState replacement)
        {
            if (QualifiedName.IsNull(browseName))
            {
                return null;
            }

            BaseInstanceState instance = null;

            switch (browseName.Name)
            {
                case BrowseNames.Output:
                    {
                        if (createOrReplace)
                        {
                            if (Output == null)
                            {
                                if (replacement == null)
                                {
                                    Output = new AnalogItemState<double>(this);
                                }
                                else
                                {
                                    Output = (AnalogItemState<double>)replacement;
                                }
                            }
                        }

                        instance = Output;
                        break;
                    }

                case BrowseNames.Units:
                    {
                        if (createOrReplace)
                        {
                            if (Units == null)
                            {
                                if (replacement == null)
                                {
                                    Units = new PropertyState<string>(this);
                                }
                                else
                                {
                                    Units = (PropertyState<string>)replacement;
                                }
                            }
                        }

                        instance = Units;
                        break;
                    }
            }

            if (instance != null)
            {
                return instance;
            }

            return base.FindChild(context, browseName, createOrReplace, replacement);
        }
        #endregion

        #region Private Fields
        private AnalogItemState<double> m_output;
        private PropertyState<string> m_units;
        #endregion
    }
#endif
    #endregion

    #region GenericActuatorState Class
#if (!OPCUA_EXCLUDE_GenericActuatorState)
    /// <summary>
    /// Stores an instance of the GenericActuatorType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCode("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class GenericActuatorState : BaseObjectState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public GenericActuatorState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(ObjectTypes.GenericActuatorType, Namespaces.BatchPlant, namespaceUris);
        }

#if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the instance with a node.
        /// </summary>
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AgAAACAAAAAKCQkJaHR0cDovL29wY2ZvdW5kYXRpb24ub3JnL1VBLyMAAABodHRwOi8vb3BjZm91bmRh" +
           "dGlvbi5vcmcvQmF0Y2hQbGFudP////8EYIAAAQAAAAIAGwAAAEdlbmVyaWNBY3R1YXRvclR5cGVJbnN0" +
           "YW5jZQECqDoBAqg6/////wIAAAAVYIkKAgAAAAIABQAAAElucHV0AQKpOgAvAQBACak6AAAAC/////8D" +
           "A/////8BAAAAFWCJCgIAAAAAAAcAAABFVVJhbmdlAQKtOgAuAEStOgAAAQB0A/////8BAf////8AAAAA" +
           "FWCJCgIAAAACAAYAAABPdXRwdXQBAq86AC8BAEAJrzoAAAAL/////wMD/////wEAAAAVYIkKAgAAAAAA" +
           "BwAAAEVVUmFuZ2UBArM6AC4ARLM6AAABAHQD/////wEB/////wAAAAA=";
        #endregion
#endif
        #endregion

        #region Public Properties
        /// <summary>
        /// A description for the Input Variable.
        /// </summary>
        public AnalogItemState<double> Input
        {
            get
            {
                return m_input;
            }

            set
            {
                if (!object.ReferenceEquals(m_input, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_input = value;
            }
        }

        /// <summary>
        /// A description for the Output Variable.
        /// </summary>
        public AnalogItemState<double> Output
        {
            get
            {
                return m_output;
            }

            set
            {
                if (!object.ReferenceEquals(m_output, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_output = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Populates a list with the children that belong to the node.
        /// </summary>
        /// <param name="context">The context for the system being accessed.</param>
        /// <param name="children">The list of children to populate.</param>
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_input != null)
            {
                children.Add(m_input);
            }

            if (m_output != null)
            {
                children.Add(m_output);
            }

            base.GetChildren(context, children);
        }

        /// <summary>
        /// Finds the child with the specified browse name.
        /// </summary>
        protected override BaseInstanceState FindChild(
            ISystemContext context,
            QualifiedName browseName,
            bool createOrReplace,
            BaseInstanceState replacement)
        {
            if (QualifiedName.IsNull(browseName))
            {
                return null;
            }

            BaseInstanceState instance = null;

            switch (browseName.Name)
            {
                case BrowseNames.Input:
                    {
                        if (createOrReplace)
                        {
                            if (Input == null)
                            {
                                if (replacement == null)
                                {
                                    Input = new AnalogItemState<double>(this);
                                }
                                else
                                {
                                    Input = (AnalogItemState<double>)replacement;
                                }
                            }
                        }

                        instance = Input;
                        break;
                    }

                case BrowseNames.Output:
                    {
                        if (createOrReplace)
                        {
                            if (Output == null)
                            {
                                if (replacement == null)
                                {
                                    Output = new AnalogItemState<double>(this);
                                }
                                else
                                {
                                    Output = (AnalogItemState<double>)replacement;
                                }
                            }
                        }

                        instance = Output;
                        break;
                    }
            }

            if (instance != null)
            {
                return instance;
            }

            return base.FindChild(context, browseName, createOrReplace, replacement);
        }
        #endregion

        #region Private Fields
        private AnalogItemState<double> m_input;
        private AnalogItemState<double> m_output;
        #endregion
    }
#endif
    #endregion

    #region LoadcellTransmitterState Class
#if (!OPCUA_EXCLUDE_LoadcellTransmitterState)
    /// <summary>
    /// Stores an instance of the LoadcellTransmitterType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCode("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class LoadcellTransmitterState : GenericSensorState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public LoadcellTransmitterState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(ObjectTypes.LoadcellTransmitterType, Namespaces.BatchPlant, namespaceUris);
        }

#if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the instance with a node.
        /// </summary>
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AgAAACAAAAAKCQkJaHR0cDovL29wY2ZvdW5kYXRpb24ub3JnL1VBLyMAAABodHRwOi8vb3BjZm91bmRh" +
           "dGlvbi5vcmcvQmF0Y2hQbGFudP////8EYIAAAQAAAAIAHwAAAExvYWRjZWxsVHJhbnNtaXR0ZXJUeXBl" +
           "SW5zdGFuY2UBArU6AQK1Ov////8DAAAAFWCJCgIAAAACAAYAAABPdXRwdXQBArY6AC8BAEAJtjoAAAAL" +
           "/////wEB/////wEAAAAVYIkKAgAAAAAABwAAAEVVUmFuZ2UBAro6AC4ARLo6AAABAHQD/////wEB////" +
           "/wAAAAAVYIkKAgAAAAIABQAAAFVuaXRzAQK8OgAuAES8OgAAAAz/////AwP/////AAAAABVgiQoCAAAA" +
           "AgARAAAARXhjaXRhdGlvblZvbHRhZ2UBAr06AC8BAEAJvToAAAAL/////wMD/////wEAAAAVYIkKAgAA" +
           "AAAABwAAAEVVUmFuZ2UBAsE6AC4ARME6AAABAHQD/////wEB/////wAAAAA=";
        #endregion
#endif
        #endregion

        #region Public Properties
        /// <summary>
        /// A description for the ExcitationVoltage Variable.
        /// </summary>
        public AnalogItemState<double> ExcitationVoltage
        {
            get
            {
                return m_excitationVoltage;
            }

            set
            {
                if (!object.ReferenceEquals(m_excitationVoltage, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_excitationVoltage = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Populates a list with the children that belong to the node.
        /// </summary>
        /// <param name="context">The context for the system being accessed.</param>
        /// <param name="children">The list of children to populate.</param>
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_excitationVoltage != null)
            {
                children.Add(m_excitationVoltage);
            }

            base.GetChildren(context, children);
        }

        /// <summary>
        /// Finds the child with the specified browse name.
        /// </summary>
        protected override BaseInstanceState FindChild(
            ISystemContext context,
            QualifiedName browseName,
            bool createOrReplace,
            BaseInstanceState replacement)
        {
            if (QualifiedName.IsNull(browseName))
            {
                return null;
            }

            BaseInstanceState instance = null;

            switch (browseName.Name)
            {
                case BrowseNames.ExcitationVoltage:
                    {
                        if (createOrReplace)
                        {
                            if (ExcitationVoltage == null)
                            {
                                if (replacement == null)
                                {
                                    ExcitationVoltage = new AnalogItemState<double>(this);
                                }
                                else
                                {
                                    ExcitationVoltage = (AnalogItemState<double>)replacement;
                                }
                            }
                        }

                        instance = ExcitationVoltage;
                        break;
                    }
            }

            if (instance != null)
            {
                return instance;
            }

            return base.FindChild(context, browseName, createOrReplace, replacement);
        }
        #endregion

        #region Private Fields
        private AnalogItemState<double> m_excitationVoltage;
        #endregion
    }
#endif
    #endregion

    #region ValveState Class
#if (!OPCUA_EXCLUDE_ValveState)
    /// <summary>
    /// Stores an instance of the ValveType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCode("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class ValveState : GenericActuatorState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public ValveState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(ObjectTypes.ValveType, Namespaces.BatchPlant, namespaceUris);
        }

#if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the instance with a node.
        /// </summary>
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AgAAACAAAAAKCQkJaHR0cDovL29wY2ZvdW5kYXRpb24ub3JnL1VBLyMAAABodHRwOi8vb3BjZm91bmRh" +
           "dGlvbi5vcmcvQmF0Y2hQbGFudP////8EYIAAAQAAAAIAEQAAAFZhbHZlVHlwZUluc3RhbmNlAQLDOgEC" +
           "wzr/////AgAAABVgiQoCAAAAAgAFAAAASW5wdXQBAsQ6AC8BAEAJxDoAAAAL/////wMD/////wEAAAAV" +
           "YIkKAgAAAAAABwAAAEVVUmFuZ2UBAsg6AC4ARMg6AAABAHQD/////wEB/////wAAAAAVYIkKAgAAAAIA" +
           "BgAAAE91dHB1dAECyjoALwEAQAnKOgAAAAv/////AwP/////AQAAABVgiQoCAAAAAAAHAAAARVVSYW5n" +
           "ZQECzjoALgBEzjoAAAEAdAP/////AQH/////AAAAAA==";
        #endregion
#endif
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        #endregion

        #region Private Fields
        #endregion
    }
#endif
    #endregion

    #region MixerMotorState Class
#if (!OPCUA_EXCLUDE_MixerMotorState)
    /// <summary>
    /// Stores an instance of the MixerMotorType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCode("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class MixerMotorState : GenericMotorState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public MixerMotorState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(ObjectTypes.MixerMotorType, Namespaces.BatchPlant, namespaceUris);
        }

#if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the instance with a node.
        /// </summary>
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AgAAACAAAAAKCQkJaHR0cDovL29wY2ZvdW5kYXRpb24ub3JnL1VBLyMAAABodHRwOi8vb3BjZm91bmRh" +
           "dGlvbi5vcmcvQmF0Y2hQbGFudP////8EYIAAAQAAAAIAFgAAAE1peGVyTW90b3JUeXBlSW5zdGFuY2UB" +
           "AtA6AQLQOv////8BAAAAFWCJCgIAAAACAAUAAABTcGVlZAEC0ToALwEAQAnROgAAAAv/////AwP/////" +
           "AQAAABVgiQoCAAAAAAAHAAAARVVSYW5nZQEC1ToALgBE1ToAAAEAdAP/////AQH/////AAAAAA==";
        #endregion
#endif
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        #endregion

        #region Private Fields
        #endregion
    }
#endif
    #endregion

    #region MixerState Class
#if (!OPCUA_EXCLUDE_MixerState)
    /// <summary>
    /// Stores an instance of the MixerType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCode("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class MixerState : FolderState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public MixerState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(ObjectTypes.MixerType, Namespaces.BatchPlant, namespaceUris);
        }

#if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the instance with a node.
        /// </summary>
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AgAAACAAAAAKCQkJaHR0cDovL29wY2ZvdW5kYXRpb24ub3JnL1VBLyMAAABodHRwOi8vb3BjZm91bmRh" +
           "dGlvbi5vcmcvQmF0Y2hQbGFudP////8EYIAAAQAAAAIAEQAAAE1peGVyVHlwZUluc3RhbmNlAQLXOgEC" +
           "1zoBAAAAADAAAQLYOgMAAACEYMAKAQAAABMAAABMb2FkY2VsbFRyYW5zbWl0dGVyAgAFAAAATFQwMDEB" +
           "Atg6AC8BArU62DoAAAEBAAAAADABAQLXOgMAAAAVYIkKAgAAAAIABgAAAE91dHB1dAEC2ToALwEAQAnZ" +
           "OgAAAAv/////AQH/////AQAAABVgiQoCAAAAAAAHAAAARVVSYW5nZQEC3ToALgBE3ToAAAEAdAP/////" +
           "AQH/////AAAAABVgiQoCAAAAAgAFAAAAVW5pdHMBAt86AC4ARN86AAAADP////8DA/////8AAAAAFWCJ" +
           "CgIAAAACABEAAABFeGNpdGF0aW9uVm9sdGFnZQEC4DoALwEAQAngOgAAAAv/////AwP/////AQAAABVg" +
           "iQoCAAAAAAAHAAAARVVSYW5nZQEC5DoALgBE5DoAAAEAdAP/////AQH/////AAAAAIRgwAoBAAAACgAA" +
           "AE1peGVyTW90b3ICAAcAAABNb3RvcjAxAQLmOgAvAQLQOuY6AAAB/////wEAAAAVYIkKAgAAAAIABQAA" +
           "AFNwZWVkAQLnOgAvAQBACec6AAAAC/////8DA/////8BAAAAFWCJCgIAAAAAAAcAAABFVVJhbmdlAQLr" +
           "OgAuAETrOgAAAQB0A/////8BAf////8AAAAAhGDACgEAAAATAAAATWl4ZXJEaXNjaGFyZ2VWYWx2ZQIA" +
           "CAAAAFZhbHZlMDA0AQLtOgAvAQLDOu06AAAB/////wIAAAAVYIkKAgAAAAIABQAAAElucHV0AQLuOgAv" +
           "AQBACe46AAAAC/////8DA/////8BAAAAFWCJCgIAAAAAAAcAAABFVVJhbmdlAQLyOgAuAETyOgAAAQB0" +
           "A/////8BAf////8AAAAAFWCJCgIAAAACAAYAAABPdXRwdXQBAvQ6AC8BAEAJ9DoAAAAL/////wMD////" +
           "/wEAAAAVYIkKAgAAAAAABwAAAEVVUmFuZ2UBAvg6AC4ARPg6AAABAHQD/////wEB/////wAAAAA=";
        #endregion
#endif
        #endregion

        #region Public Properties
        /// <summary>
        /// A description for the LT001 Object.
        /// </summary>
        public LoadcellTransmitterState LoadcellTransmitter
        {
            get
            {
                return m_loadcellTransmitter;
            }

            set
            {
                if (!ReferenceEquals(m_loadcellTransmitter, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_loadcellTransmitter = value;
            }
        }

        /// <summary>
        /// A description for the Motor01 Object.
        /// </summary>
        public MixerMotorState MixerMotor
        {
            get
            {
                return m_mixerMotor;
            }

            set
            {
                if (!ReferenceEquals(m_mixerMotor, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_mixerMotor = value;
            }
        }

        /// <summary>
        /// A description for the Valve004 Object.
        /// </summary>
        public ValveState MixerDischargeValve
        {
            get
            {
                return m_mixerDischargeValve;
            }

            set
            {
                if (!ReferenceEquals(m_mixerDischargeValve, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_mixerDischargeValve = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Populates a list with the children that belong to the node.
        /// </summary>
        /// <param name="context">The context for the system being accessed.</param>
        /// <param name="children">The list of children to populate.</param>
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_loadcellTransmitter != null)
            {
                children.Add(m_loadcellTransmitter);
            }

            if (m_mixerMotor != null)
            {
                children.Add(m_mixerMotor);
            }

            if (m_mixerDischargeValve != null)
            {
                children.Add(m_mixerDischargeValve);
            }

            base.GetChildren(context, children);
        }

        /// <summary>
        /// Finds the child with the specified browse name.
        /// </summary>
        protected override BaseInstanceState FindChild(
            ISystemContext context,
            QualifiedName browseName,
            bool createOrReplace,
            BaseInstanceState replacement)
        {
            if (QualifiedName.IsNull(browseName))
            {
                return null;
            }

            BaseInstanceState instance = null;

            switch (browseName.Name)
            {
                case BrowseNames.LoadcellTransmitter:
                    {
                        if (createOrReplace)
                        {
                            if (LoadcellTransmitter == null)
                            {
                                if (replacement == null)
                                {
                                    LoadcellTransmitter = new LoadcellTransmitterState(this);
                                }
                                else
                                {
                                    LoadcellTransmitter = (LoadcellTransmitterState)replacement;
                                }
                            }
                        }

                        instance = LoadcellTransmitter;
                        break;
                    }

                case BrowseNames.MixerMotor:
                    {
                        if (createOrReplace)
                        {
                            if (MixerMotor == null)
                            {
                                if (replacement == null)
                                {
                                    MixerMotor = new MixerMotorState(this);
                                }
                                else
                                {
                                    MixerMotor = (MixerMotorState)replacement;
                                }
                            }
                        }

                        instance = MixerMotor;
                        break;
                    }

                case BrowseNames.MixerDischargeValve:
                    {
                        if (createOrReplace)
                        {
                            if (MixerDischargeValve == null)
                            {
                                if (replacement == null)
                                {
                                    MixerDischargeValve = new ValveState(this);
                                }
                                else
                                {
                                    MixerDischargeValve = (ValveState)replacement;
                                }
                            }
                        }

                        instance = MixerDischargeValve;
                        break;
                    }
            }

            if (instance != null)
            {
                return instance;
            }

            return base.FindChild(context, browseName, createOrReplace, replacement);
        }
        #endregion

        #region Private Fields
        private LoadcellTransmitterState m_loadcellTransmitter;
        private MixerMotorState m_mixerMotor;
        private ValveState m_mixerDischargeValve;
        #endregion
    }
#endif
    #endregion

    #region BatchPlantState Class
#if (!OPCUA_EXCLUDE_BatchPlantState)
    /// <summary>
    /// Stores an instance of the BatchPlantType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCode("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class BatchPlantState : BaseObjectState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public BatchPlantState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(ObjectTypes.BatchPlantType, Namespaces.BatchPlant, namespaceUris);
        }

#if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the instance with a node.
        /// </summary>
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AgAAACAAAAAKCQkJaHR0cDovL29wY2ZvdW5kYXRpb24ub3JnL1VBLyMAAABodHRwOi8vb3BjZm91bmRh" +
           "dGlvbi5vcmcvQmF0Y2hQbGFudP////+EYIAAAQAAAAIAFgAAAEJhdGNoUGxhbnRUeXBlSW5zdGFuY2UB" +
           "Avo6AQL6OgEBAAAAADAAAQL7OgMAAACEYMAKAQAAAAUAAABNaXhlcgIACQAAAE1peGVyWDAwMQEC+zoA" +
           "LwEC1zr7OgAAAQIAAAAAMAEBAvo6ADAAAQL8OgQAAACEYMAKAQAAABMAAABMb2FkY2VsbFRyYW5zbWl0" +
           "dGVyAgAFAAAATFQwMDEBAvw6AC8BArU6/DoAAAEBAAAAADABAQL7OgMAAAAVYIkKAgAAAAIABgAAAE91" +
           "dHB1dAEC/ToALwEAQAn9OgAAAAv/////AQH/////AQAAABVgiQoCAAAAAAAHAAAARVVSYW5nZQECATsA" +
           "LgBEATsAAAEAdAP/////AQH/////AAAAABVgiQoCAAAAAgAFAAAAVW5pdHMBAgM7AC4ARAM7AAAADP//" +
           "//8DA/////8AAAAAFWCJCgIAAAACABEAAABFeGNpdGF0aW9uVm9sdGFnZQECBDsALwEAQAkEOwAAAAv/" +
           "////AwP/////AQAAABVgiQoCAAAAAAAHAAAARVVSYW5nZQECCDsALgBECDsAAAEAdAP/////AQH/////" +
           "AAAAAIRgwAoBAAAACgAAAE1peGVyTW90b3ICAAcAAABNb3RvcjAxAQIKOwAvAQLQOgo7AAAB/////wEA" +
           "AAAVYIkKAgAAAAIABQAAAFNwZWVkAQILOwAvAQBACQs7AAAAC/////8DA/////8BAAAAFWCJCgIAAAAA" +
           "AAcAAABFVVJhbmdlAQIPOwAuAEQPOwAAAQB0A/////8BAf////8AAAAAhGDACgEAAAATAAAATWl4ZXJE" +
           "aXNjaGFyZ2VWYWx2ZQIACAAAAFZhbHZlMDA0AQIROwAvAQLDOhE7AAAB/////wIAAAAVYIkKAgAAAAIA" +
           "BQAAAElucHV0AQISOwAvAQBACRI7AAAAC/////8DA/////8BAAAAFWCJCgIAAAAAAAcAAABFVVJhbmdl" +
           "AQIWOwAuAEQWOwAAAQB0A/////8BAf////8AAAAAFWCJCgIAAAACAAYAAABPdXRwdXQBAhg7AC8BAEAJ" +
           "GDsAAAAL/////wMD/////wEAAAAVYIkKAgAAAAAABwAAAEVVUmFuZ2UBAhw7AC4ARBw7AAABAHQD////" +
           "/wEB/////wAAAAAEYIAKAQAAAAIADgAAAERpc2NoYXJnZVZhbHZlAQIeOwAvADoeOwAA/////wEAAAAV" +
           "YIkKAgAAAAIABQAAAElucHV0AQIfOwAvAD8fOwAAABj/////AQH/////AAAAAARhggoEAAAAAgAMAAAA" +
           "U3RhcnRQcm9jZXNzAQIgOwAvAQIgOyA7AAABAf////8AAAAABGGCCgQAAAACAAsAAABTdG9wUHJvY2Vz" +
           "cwECITsALwECITshOwAAAQH/////AAAAAA==";
        #endregion
#endif
        #endregion

        #region Public Properties
        /// <summary>
        /// A description for the MixerX001 Object.
        /// </summary>
        public MixerState Mixer
        {
            get
            {
                return m_mixer;
            }

            set
            {
                if (!ReferenceEquals(m_mixer, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_mixer = value;
            }
        }

        /// <summary>
        /// A description for the StartProcess Method.
        /// </summary>
        public MethodState StartProcess
        {
            get
            {
                return m_startProcessMethod;
            }

            set
            {
                if (!object.ReferenceEquals(m_startProcessMethod, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_startProcessMethod = value;
            }
        }

        /// <summary>
        /// A description for the StopProcess Method.
        /// </summary>
        public MethodState StopProcess
        {
            get
            {
                return m_stopProcessMethod;
            }

            set
            {
                if (!object.ReferenceEquals(m_stopProcessMethod, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_stopProcessMethod = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Populates a list with the children that belong to the node.
        /// </summary>
        /// <param name="context">The context for the system being accessed.</param>
        /// <param name="children">The list of children to populate.</param>
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_mixer != null)
            {
                children.Add(m_mixer);
            }

            if (m_startProcessMethod != null)
            {
                children.Add(m_startProcessMethod);
            }

            if (m_stopProcessMethod != null)
            {
                children.Add(m_stopProcessMethod);
            }

            base.GetChildren(context, children);
        }

        /// <summary>
        /// Finds the child with the specified browse name.
        /// </summary>
        protected override BaseInstanceState FindChild(
            ISystemContext context,
            QualifiedName browseName,
            bool createOrReplace,
            BaseInstanceState replacement)
        {
            if (QualifiedName.IsNull(browseName))
            {
                return null;
            }

            BaseInstanceState instance = null;

            switch (browseName.Name)
            {
                case BrowseNames.Mixer:
                    {
                        if (createOrReplace)
                        {
                            if (Mixer == null)
                            {
                                if (replacement == null)
                                {
                                    Mixer = new MixerState(this);
                                }
                                else
                                {
                                    Mixer = (MixerState)replacement;
                                }
                            }
                        }

                        instance = Mixer;
                        break;
                    }

                case BrowseNames.StartProcess:
                    {
                        if (createOrReplace)
                        {
                            if (StartProcess == null)
                            {
                                if (replacement == null)
                                {
                                    StartProcess = new MethodState(this);
                                }
                                else
                                {
                                    StartProcess = (MethodState)replacement;
                                }
                            }
                        }

                        instance = StartProcess;
                        break;
                    }

                case BrowseNames.StopProcess:
                    {
                        if (createOrReplace)
                        {
                            if (StopProcess == null)
                            {
                                if (replacement == null)
                                {
                                    StopProcess = new MethodState(this);
                                }
                                else
                                {
                                    StopProcess = (MethodState)replacement;
                                }
                            }
                        }

                        instance = StopProcess;
                        break;
                    }
            }

            if (instance != null)
            {
                return instance;
            }

            return base.FindChild(context, browseName, createOrReplace, replacement);
        }
        #endregion

        #region Private Fields
        private MixerState m_mixer;
        private MethodState m_startProcessMethod;
        private MethodState m_stopProcessMethod;
        #endregion
    }
#endif
    #endregion
}
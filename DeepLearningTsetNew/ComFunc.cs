using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepLearningTset
{
    static public class ComFunc
    {


        // Procedures 
        // Chapter: Deep Learning / Model
        // Short Description: Create a dictionary with the preprocessing parameters based on a given DL model. 
        static public void create_dl_preprocess_param_from_model(HTuple hv_DLModelHandle, HTuple hv_NormalizationType,
            HTuple hv_DomainHandling, HTuple hv_SetBackgroundID, HTuple hv_ClassIDsBackground,
            HTuple hv_GenParam, out HTuple hv_DLPreprocessParam)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_ModelType = new HTuple(), hv_ImageWidth = new HTuple();
            HTuple hv_ImageHeight = new HTuple(), hv_ImageNumChannels = new HTuple();
            HTuple hv_ImageRangeMin = new HTuple(), hv_ImageRangeMax = new HTuple();
            HTuple hv_IgnoreClassIDs = new HTuple(), hv_InstanceType = new HTuple();
            HTuple hv_IsInstanceSegmentation = new HTuple(), hv_IgnoreDirection = new HTuple();
            HTuple hv_ClassIDsNoOrientation = new HTuple();
            HTuple hv_GenParam_COPY_INP_TMP = new HTuple(hv_GenParam);

            // Initialize local and output iconic variables 
            hv_DLPreprocessParam = new HTuple();
            //
            //This procedure creates a dictionary with all parameters needed for preprocessing
            //according to a model provided through DLModelHandle.
            //
            //Get the relevant model parameters.
            hv_ModelType.Dispose();
            HOperatorSet.GetDlModelParam(hv_DLModelHandle, "type", out hv_ModelType);
            hv_ImageWidth.Dispose();
            HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_width", out hv_ImageWidth);
            hv_ImageHeight.Dispose();
            HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_height", out hv_ImageHeight);
            hv_ImageNumChannels.Dispose();
            HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_num_channels", out hv_ImageNumChannels);
            hv_ImageRangeMin.Dispose();
            HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_range_min", out hv_ImageRangeMin);
            hv_ImageRangeMax.Dispose();
            HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_range_max", out hv_ImageRangeMax);
            hv_IgnoreClassIDs.Dispose();
            hv_IgnoreClassIDs = new HTuple();
            //
            //Get model specific parameters.
            if ((int)((new HTuple(hv_ModelType.TupleEqual("anomaly_detection"))).TupleOr(
                new HTuple(hv_ModelType.TupleEqual("gc_anomaly_detection")))) != 0)
            {
                //No specific parameters for both anomaly detection
                //and Global Context Anomaly Detection model types.
            }
            else if ((int)(new HTuple(hv_ModelType.TupleEqual("classification"))) != 0)
            {
                //No classification specific parameters.
            }
            else if ((int)(new HTuple(hv_ModelType.TupleEqual("detection"))) != 0)
            {
                //Get detection specific parameters.
                //If GenParam has not been created yet, create it to add new generic parameters.
                if ((int)(new HTuple((new HTuple(hv_GenParam_COPY_INP_TMP.TupleLength())).TupleEqual(
                    0))) != 0)
                {
                    hv_GenParam_COPY_INP_TMP.Dispose();
                    HOperatorSet.CreateDict(out hv_GenParam_COPY_INP_TMP);
                }
                //Add instance_type.
                hv_InstanceType.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "instance_type", out hv_InstanceType);
                //If the model can do instance segmentation, the preprocessing instance_type
                //needs to be 'mask'.
                hv_IsInstanceSegmentation.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "instance_segmentation", out hv_IsInstanceSegmentation);
                if ((int)(new HTuple(hv_IsInstanceSegmentation.TupleEqual("true"))) != 0)
                {
                    HOperatorSet.SetDictTuple(hv_GenParam_COPY_INP_TMP, "instance_type", "mask");
                }
                else
                {
                    HOperatorSet.SetDictTuple(hv_GenParam_COPY_INP_TMP, "instance_type", hv_InstanceType);
                }
                //For instance_type 'rectangle2', add the boolean ignore_direction and class IDs without orientation.
                if ((int)(new HTuple(hv_InstanceType.TupleEqual("rectangle2"))) != 0)
                {
                    hv_IgnoreDirection.Dispose();
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "ignore_direction", out hv_IgnoreDirection);
                    if ((int)(new HTuple(hv_IgnoreDirection.TupleEqual("true"))) != 0)
                    {
                        HOperatorSet.SetDictTuple(hv_GenParam_COPY_INP_TMP, "ignore_direction",
                            1);
                    }
                    else if ((int)(new HTuple(hv_IgnoreDirection.TupleEqual("false"))) != 0)
                    {
                        HOperatorSet.SetDictTuple(hv_GenParam_COPY_INP_TMP, "ignore_direction",
                            0);
                    }
                    hv_ClassIDsNoOrientation.Dispose();
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_ids_no_orientation",
                        out hv_ClassIDsNoOrientation);
                    HOperatorSet.SetDictTuple(hv_GenParam_COPY_INP_TMP, "class_ids_no_orientation",
                        hv_ClassIDsNoOrientation);
                }
            }
            else if ((int)(new HTuple(hv_ModelType.TupleEqual("segmentation"))) != 0)
            {
                //Get segmentation specific parameters.
                hv_IgnoreClassIDs.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "ignore_class_ids", out hv_IgnoreClassIDs);
            }
            else if ((int)(new HTuple(hv_ModelType.TupleEqual("3d_gripping_point_detection"))) != 0)
            {
                //The input image is expected to be a single channel image.
                hv_ImageNumChannels.Dispose();
                hv_ImageNumChannels = 1;
            }
            //
            //Create the dictionary with the preprocessing parameters returned by this procedure.
            hv_DLPreprocessParam.Dispose();
            create_dl_preprocess_param(hv_ModelType, hv_ImageWidth, hv_ImageHeight, hv_ImageNumChannels,
                hv_ImageRangeMin, hv_ImageRangeMax, hv_NormalizationType, hv_DomainHandling,
                hv_IgnoreClassIDs, hv_SetBackgroundID, hv_ClassIDsBackground, hv_GenParam_COPY_INP_TMP,
                out hv_DLPreprocessParam);
            //

            hv_GenParam_COPY_INP_TMP.Dispose();
            hv_ModelType.Dispose();
            hv_ImageWidth.Dispose();
            hv_ImageHeight.Dispose();
            hv_ImageNumChannels.Dispose();
            hv_ImageRangeMin.Dispose();
            hv_ImageRangeMax.Dispose();
            hv_IgnoreClassIDs.Dispose();
            hv_InstanceType.Dispose();
            hv_IsInstanceSegmentation.Dispose();
            hv_IgnoreDirection.Dispose();
            hv_ClassIDsNoOrientation.Dispose();

            return;
        }


        // Procedures 
        // Chapter: Deep Learning / Model
        // Short Description: Create a dictionary with preprocessing parameters. 
        static public void create_dl_preprocess_param(HTuple hv_DLModelType, HTuple hv_ImageWidth,
            HTuple hv_ImageHeight, HTuple hv_ImageNumChannels, HTuple hv_ImageRangeMin,
            HTuple hv_ImageRangeMax, HTuple hv_NormalizationType, HTuple hv_DomainHandling,
            HTuple hv_IgnoreClassIDs, HTuple hv_SetBackgroundID, HTuple hv_ClassIDsBackground,
            HTuple hv_GenParam, out HTuple hv_DLPreprocessParam)
        {



            // Local control variables 

            HTuple hv_GenParamNames = new HTuple(), hv_GenParamIndex = new HTuple();
            HTuple hv_GenParamValue = new HTuple(), hv_KeysExist = new HTuple();
            HTuple hv_InstanceType = new HTuple(), hv_IsInstanceSegmentation = new HTuple();
            // Initialize local and output iconic variables 
            hv_DLPreprocessParam = new HTuple();
            try
            {
                //
                //This procedure creates a dictionary with all parameters needed for preprocessing.
                //
                hv_DLPreprocessParam.Dispose();
                HOperatorSet.CreateDict(out hv_DLPreprocessParam);
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "model_type", hv_DLModelType);
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_width", hv_ImageWidth);
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_height", hv_ImageHeight);
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_num_channels", hv_ImageNumChannels);
                if ((int)(new HTuple(hv_ImageRangeMin.TupleEqual(new HTuple()))) != 0)
                {
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_range_min", -127);
                }
                else
                {
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_range_min", hv_ImageRangeMin);
                }
                if ((int)(new HTuple(hv_ImageRangeMax.TupleEqual(new HTuple()))) != 0)
                {
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_range_max", 128);
                }
                else
                {
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_range_max", hv_ImageRangeMax);
                }
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "normalization_type", hv_NormalizationType);
                //Replace possible legacy parameters.
                replace_legacy_preprocessing_parameters(hv_DLPreprocessParam);
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "domain_handling", hv_DomainHandling);
                //
                //Set segmentation specific parameters.
                if ((int)(new HTuple(hv_DLModelType.TupleEqual("segmentation"))) != 0)
                {
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "ignore_class_ids", hv_IgnoreClassIDs);
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "set_background_id", hv_SetBackgroundID);
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "class_ids_background", hv_ClassIDsBackground);
                }
                //
                //Set default values of generic parameters.
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "augmentation", "false");
                //
                //Set generic parameters.
                if ((int)(new HTuple(hv_GenParam.TupleNotEqual(new HTuple()))) != 0)
                {
                    hv_GenParamNames.Dispose();
                    HOperatorSet.GetDictParam(hv_GenParam, "keys", new HTuple(), out hv_GenParamNames);
                    for (hv_GenParamIndex = 0; (int)hv_GenParamIndex <= (int)((new HTuple(hv_GenParamNames.TupleLength()
                        )) - 1); hv_GenParamIndex = (int)hv_GenParamIndex + 1)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_GenParamValue.Dispose();
                            HOperatorSet.GetDictTuple(hv_GenParam, hv_GenParamNames.TupleSelect(hv_GenParamIndex),
                                out hv_GenParamValue);
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_DLPreprocessParam, hv_GenParamNames.TupleSelect(
                                hv_GenParamIndex), hv_GenParamValue);
                        }
                    }
                }
                //
                //Set necessary default values.
                if ((int)(new HTuple(hv_DLModelType.TupleEqual("detection"))) != 0)
                {
                    hv_KeysExist.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", ((new HTuple("instance_type")).TupleConcat(
                        "ignore_direction")).TupleConcat("instance_segmentation"), out hv_KeysExist);
                    if ((int)(((hv_KeysExist.TupleSelect(0))).TupleNot()) != 0)
                    {
                        HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "instance_type", "rectangle1");
                    }
                    //Set default for 'ignore_direction' only if instance_type is 'rectangle2'.
                    hv_InstanceType.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "instance_type", out hv_InstanceType);
                    if ((int)((new HTuple(hv_InstanceType.TupleEqual("rectangle2"))).TupleAnd(
                        ((hv_KeysExist.TupleSelect(1))).TupleNot())) != 0)
                    {
                        HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "ignore_direction", 0);
                    }
                    //In case of instance_segmentation we overwrite the instance_type to mask.
                    if ((int)(hv_KeysExist.TupleSelect(2)) != 0)
                    {
                        hv_IsInstanceSegmentation.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "instance_segmentation",
                            out hv_IsInstanceSegmentation);
                        if ((int)(new HTuple((new HTuple(((((new HTuple(1)).TupleConcat(0)).TupleConcat(
                            "true")).TupleConcat("false")).TupleFind(hv_IsInstanceSegmentation))).TupleEqual(
                            -1))) != 0)
                        {
                            throw new HalconException(("Invalid generic parameter for 'instance_segmentation': " + hv_IsInstanceSegmentation) + new HTuple(", only true and false are allowed"));
                        }
                        if ((int)((new HTuple(hv_IsInstanceSegmentation.TupleEqual("true"))).TupleOr(
                            new HTuple(hv_IsInstanceSegmentation.TupleEqual(1)))) != 0)
                        {
                            HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "instance_type", "mask");
                        }
                    }
                }
                //
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //

                hv_GenParamNames.Dispose();
                hv_GenParamIndex.Dispose();
                hv_GenParamValue.Dispose();
                hv_KeysExist.Dispose();
                hv_InstanceType.Dispose();
                hv_IsInstanceSegmentation.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_GenParamNames.Dispose();
                hv_GenParamIndex.Dispose();
                hv_GenParamValue.Dispose();
                hv_KeysExist.Dispose();
                hv_InstanceType.Dispose();
                hv_IsInstanceSegmentation.Dispose();

                throw HDevExpDefaultException;
            }
        }





        // Procedures 
        // Chapter: Deep Learning / Model
        // Short Description: Replace legacy preprocessing parameters or values. 
        static private void replace_legacy_preprocessing_parameters(HTuple hv_DLPreprocessParam)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Exception = new HTuple(), hv_NormalizationTypeExists = new HTuple();
            HTuple hv_NormalizationType = new HTuple(), hv_LegacyNormalizationKeyExists = new HTuple();
            HTuple hv_ContrastNormalization = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //
                //This procedure adapts the dictionary DLPreprocessParam
                //if a legacy preprocessing parameter is set.
                //
                //Map legacy value set to new parameter.
                hv_Exception.Dispose();
                hv_Exception = 0;
                try
                {
                    hv_NormalizationTypeExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "normalization_type",
                        out hv_NormalizationTypeExists);
                    //
                    if ((int)(hv_NormalizationTypeExists) != 0)
                    {
                        hv_NormalizationType.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "normalization_type", out hv_NormalizationType);
                        if ((int)(new HTuple(hv_NormalizationType.TupleEqual("true"))) != 0)
                        {
                            hv_NormalizationType.Dispose();
                            hv_NormalizationType = "first_channel";
                        }
                        else if ((int)(new HTuple(hv_NormalizationType.TupleEqual("false"))) != 0)
                        {
                            hv_NormalizationType.Dispose();
                            hv_NormalizationType = "none";
                        }
                        HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "normalization_type", hv_NormalizationType);
                    }
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                }
                //
                //Map legacy parameter to new parameter and corresponding value.
                hv_Exception.Dispose();
                hv_Exception = 0;
                try
                {
                    hv_LegacyNormalizationKeyExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "contrast_normalization",
                        out hv_LegacyNormalizationKeyExists);
                    if ((int)(hv_LegacyNormalizationKeyExists) != 0)
                    {
                        hv_ContrastNormalization.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "contrast_normalization",
                            out hv_ContrastNormalization);
                        //Replace 'contrast_normalization' by 'normalization_type'.
                        if ((int)(new HTuple(hv_ContrastNormalization.TupleEqual("false"))) != 0)
                        {
                            HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "normalization_type",
                                "none");
                        }
                        else if ((int)(new HTuple(hv_ContrastNormalization.TupleEqual(
                            "true"))) != 0)
                        {
                            HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "normalization_type",
                                "first_channel");
                        }
                        HOperatorSet.RemoveDictKey(hv_DLPreprocessParam, "contrast_normalization");
                    }
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                }

                hv_Exception.Dispose();
                hv_NormalizationTypeExists.Dispose();
                hv_NormalizationType.Dispose();
                hv_LegacyNormalizationKeyExists.Dispose();
                hv_ContrastNormalization.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Exception.Dispose();
                hv_NormalizationTypeExists.Dispose();
                hv_NormalizationType.Dispose();
                hv_LegacyNormalizationKeyExists.Dispose();
                hv_ContrastNormalization.Dispose();

                throw HDevExpDefaultException;
            }
        }


        // Procedures 
        // Chapter: Deep Learning / Model
        // Short Description: Check the content of the parameter dictionary DLPreprocessParam. 
        static private void check_dl_preprocess_param(HTuple hv_DLPreprocessParam)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_CheckParams = new HTuple(), hv_KeyExists = new HTuple();
            HTuple hv_DLModelType = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_SupportedModelTypes = new HTuple(), hv_Index = new HTuple();
            HTuple hv_ParamNamesGeneral = new HTuple(), hv_ParamNamesSegmentation = new HTuple();
            HTuple hv_ParamNamesDetectionOptional = new HTuple(), hv_ParamNamesPreprocessingOptional = new HTuple();
            HTuple hv_ParamNames3DGrippingPointsOptional = new HTuple();
            HTuple hv_ParamNamesAll = new HTuple(), hv_ParamNames = new HTuple();
            HTuple hv_KeysExists = new HTuple(), hv_I = new HTuple();
            HTuple hv_Exists = new HTuple(), hv_InputKeys = new HTuple();
            HTuple hv_Key = new HTuple(), hv_Value = new HTuple();
            HTuple hv_Indices = new HTuple(), hv_ValidValues = new HTuple();
            HTuple hv_ValidTypes = new HTuple(), hv_V = new HTuple();
            HTuple hv_T = new HTuple(), hv_IsInt = new HTuple(), hv_ValidTypesListing = new HTuple();
            HTuple hv_ValidValueListing = new HTuple(), hv_EmptyStrings = new HTuple();
            HTuple hv_ImageRangeMinExists = new HTuple(), hv_ImageRangeMaxExists = new HTuple();
            HTuple hv_ImageRangeMin = new HTuple(), hv_ImageRangeMax = new HTuple();
            HTuple hv_IndexParam = new HTuple(), hv_SetBackgroundID = new HTuple();
            HTuple hv_ClassIDsBackground = new HTuple(), hv_Intersection = new HTuple();
            HTuple hv_IgnoreClassIDs = new HTuple(), hv_KnownClasses = new HTuple();
            HTuple hv_IgnoreClassID = new HTuple(), hv_OptionalKeysExist = new HTuple();
            HTuple hv_InstanceType = new HTuple(), hv_IsInstanceSegmentation = new HTuple();
            HTuple hv_IgnoreDirection = new HTuple(), hv_ClassIDsNoOrientation = new HTuple();
            HTuple hv_SemTypes = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //
                //This procedure checks a dictionary with parameters for DL preprocessing.
                //
                hv_CheckParams.Dispose();
                hv_CheckParams = 1;
                //If check_params is set to false, do not check anything.
                hv_KeyExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "check_params",
                    out hv_KeyExists);
                if ((int)(hv_KeyExists) != 0)
                {
                    hv_CheckParams.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "check_params", out hv_CheckParams);
                    if ((int)(hv_CheckParams.TupleNot()) != 0)
                    {

                        hv_CheckParams.Dispose();
                        hv_KeyExists.Dispose();
                        hv_DLModelType.Dispose();
                        hv_Exception.Dispose();
                        hv_SupportedModelTypes.Dispose();
                        hv_Index.Dispose();
                        hv_ParamNamesGeneral.Dispose();
                        hv_ParamNamesSegmentation.Dispose();
                        hv_ParamNamesDetectionOptional.Dispose();
                        hv_ParamNamesPreprocessingOptional.Dispose();
                        hv_ParamNames3DGrippingPointsOptional.Dispose();
                        hv_ParamNamesAll.Dispose();
                        hv_ParamNames.Dispose();
                        hv_KeysExists.Dispose();
                        hv_I.Dispose();
                        hv_Exists.Dispose();
                        hv_InputKeys.Dispose();
                        hv_Key.Dispose();
                        hv_Value.Dispose();
                        hv_Indices.Dispose();
                        hv_ValidValues.Dispose();
                        hv_ValidTypes.Dispose();
                        hv_V.Dispose();
                        hv_T.Dispose();
                        hv_IsInt.Dispose();
                        hv_ValidTypesListing.Dispose();
                        hv_ValidValueListing.Dispose();
                        hv_EmptyStrings.Dispose();
                        hv_ImageRangeMinExists.Dispose();
                        hv_ImageRangeMaxExists.Dispose();
                        hv_ImageRangeMin.Dispose();
                        hv_ImageRangeMax.Dispose();
                        hv_IndexParam.Dispose();
                        hv_SetBackgroundID.Dispose();
                        hv_ClassIDsBackground.Dispose();
                        hv_Intersection.Dispose();
                        hv_IgnoreClassIDs.Dispose();
                        hv_KnownClasses.Dispose();
                        hv_IgnoreClassID.Dispose();
                        hv_OptionalKeysExist.Dispose();
                        hv_InstanceType.Dispose();
                        hv_IsInstanceSegmentation.Dispose();
                        hv_IgnoreDirection.Dispose();
                        hv_ClassIDsNoOrientation.Dispose();
                        hv_SemTypes.Dispose();

                        return;
                    }
                }
                //
                try
                {
                    hv_DLModelType.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "model_type", out hv_DLModelType);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    throw new HalconException(new HTuple(new HTuple("DLPreprocessParam needs the parameter: '") + "model_type") + "'");
                }
                //
                //Check for correct model type.
                hv_SupportedModelTypes.Dispose();
                hv_SupportedModelTypes = new HTuple();
                hv_SupportedModelTypes[0] = "3d_gripping_point_detection";
                hv_SupportedModelTypes[1] = "anomaly_detection";
                hv_SupportedModelTypes[2] = "classification";
                hv_SupportedModelTypes[3] = "detection";
                hv_SupportedModelTypes[4] = "gc_anomaly_detection";
                hv_SupportedModelTypes[5] = "ocr_recognition";
                hv_SupportedModelTypes[6] = "segmentation";
                hv_Index.Dispose();
                HOperatorSet.TupleFind(hv_SupportedModelTypes, hv_DLModelType, out hv_Index);
                if ((int)((new HTuple(hv_Index.TupleEqual(-1))).TupleOr(new HTuple(hv_Index.TupleEqual(
                    new HTuple())))) != 0)
                {
                    throw new HalconException(new HTuple("Only models of type '3d_gripping_point_detection', 'anomaly_detection', 'classification', 'detection', 'gc_anomaly_detection', 'ocr_recognition' or 'segmentation' are supported"));

                }
                //
                //Parameter names that are required.
                //General parameters.
                hv_ParamNamesGeneral.Dispose();
                hv_ParamNamesGeneral = new HTuple();
                hv_ParamNamesGeneral[0] = "model_type";
                hv_ParamNamesGeneral[1] = "image_width";
                hv_ParamNamesGeneral[2] = "image_height";
                hv_ParamNamesGeneral[3] = "image_num_channels";
                hv_ParamNamesGeneral[4] = "image_range_min";
                hv_ParamNamesGeneral[5] = "image_range_max";
                hv_ParamNamesGeneral[6] = "normalization_type";
                hv_ParamNamesGeneral[7] = "domain_handling";
                //Segmentation specific parameters.
                hv_ParamNamesSegmentation.Dispose();
                hv_ParamNamesSegmentation = new HTuple();
                hv_ParamNamesSegmentation[0] = "ignore_class_ids";
                hv_ParamNamesSegmentation[1] = "set_background_id";
                hv_ParamNamesSegmentation[2] = "class_ids_background";
                //Detection specific parameters.
                hv_ParamNamesDetectionOptional.Dispose();
                hv_ParamNamesDetectionOptional = new HTuple();
                hv_ParamNamesDetectionOptional[0] = "instance_type";
                hv_ParamNamesDetectionOptional[1] = "ignore_direction";
                hv_ParamNamesDetectionOptional[2] = "class_ids_no_orientation";
                hv_ParamNamesDetectionOptional[3] = "instance_segmentation";
                //Optional preprocessing parameters.
                hv_ParamNamesPreprocessingOptional.Dispose();
                hv_ParamNamesPreprocessingOptional = new HTuple();
                hv_ParamNamesPreprocessingOptional[0] = "mean_values_normalization";
                hv_ParamNamesPreprocessingOptional[1] = "deviation_values_normalization";
                hv_ParamNamesPreprocessingOptional[2] = "check_params";
                hv_ParamNamesPreprocessingOptional[3] = "augmentation";
                //3D Gripping Point Detection specific parameters.
                hv_ParamNames3DGrippingPointsOptional.Dispose();
                hv_ParamNames3DGrippingPointsOptional = new HTuple();
                hv_ParamNames3DGrippingPointsOptional[0] = "min_z";
                hv_ParamNames3DGrippingPointsOptional[1] = "max_z";
                hv_ParamNames3DGrippingPointsOptional[2] = "normal_image_width";
                hv_ParamNames3DGrippingPointsOptional[3] = "normal_image_height";
                //All parameters
                hv_ParamNamesAll.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ParamNamesAll = new HTuple();
                    hv_ParamNamesAll = hv_ParamNamesAll.TupleConcat(hv_ParamNamesGeneral, hv_ParamNamesSegmentation, hv_ParamNamesDetectionOptional, hv_ParamNames3DGrippingPointsOptional, hv_ParamNamesPreprocessingOptional);
                }
                hv_ParamNames.Dispose();
                hv_ParamNames = new HTuple(hv_ParamNamesGeneral);
                if ((int)(new HTuple(hv_DLModelType.TupleEqual("segmentation"))) != 0)
                {
                    //Extend ParamNames for models of type segmentation.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_ParamNames = hv_ParamNames.TupleConcat(
                                hv_ParamNamesSegmentation);
                            hv_ParamNames.Dispose();
                            hv_ParamNames = ExpTmpLocalVar_ParamNames;
                        }
                    }
                }
                //
                //Check if legacy parameter exist.
                //Otherwise map it to the legal parameter.
                replace_legacy_preprocessing_parameters(hv_DLPreprocessParam);
                //
                //Check that all necessary parameters are included.
                //
                hv_KeysExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", hv_ParamNames,
                    out hv_KeysExists);
                if ((int)(new HTuple(((((hv_KeysExists.TupleEqualElem(0))).TupleSum())).TupleGreater(
                    0))) != 0)
                {
                    for (hv_I = 0; (int)hv_I <= (int)(new HTuple(hv_KeysExists.TupleLength())); hv_I = (int)hv_I + 1)
                    {
                        hv_Exists.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Exists = hv_KeysExists.TupleSelect(
                                hv_I);
                        }
                        if ((int)(hv_Exists.TupleNot()) != 0)
                        {
                            throw new HalconException(("DLPreprocessParam needs the parameter: '" + (hv_ParamNames.TupleSelect(
                                hv_I))) + "'");
                        }
                    }
                }
                //
                //Check the keys provided.
                hv_InputKeys.Dispose();
                HOperatorSet.GetDictParam(hv_DLPreprocessParam, "keys", new HTuple(), out hv_InputKeys);
                for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_InputKeys.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                {
                    hv_Key.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Key = hv_InputKeys.TupleSelect(
                            hv_I);
                    }
                    hv_Value.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_Key, out hv_Value);
                    //Check that the key is known.
                    hv_Indices.Dispose();
                    HOperatorSet.TupleFind(hv_ParamNamesAll, hv_Key, out hv_Indices);
                    if ((int)(new HTuple(hv_Indices.TupleEqual(-1))) != 0)
                    {
                        throw new HalconException(("Unknown key for DLPreprocessParam: '" + (hv_InputKeys.TupleSelect(
                            hv_I))) + "'");

                        hv_CheckParams.Dispose();
                        hv_KeyExists.Dispose();
                        hv_DLModelType.Dispose();
                        hv_Exception.Dispose();
                        hv_SupportedModelTypes.Dispose();
                        hv_Index.Dispose();
                        hv_ParamNamesGeneral.Dispose();
                        hv_ParamNamesSegmentation.Dispose();
                        hv_ParamNamesDetectionOptional.Dispose();
                        hv_ParamNamesPreprocessingOptional.Dispose();
                        hv_ParamNames3DGrippingPointsOptional.Dispose();
                        hv_ParamNamesAll.Dispose();
                        hv_ParamNames.Dispose();
                        hv_KeysExists.Dispose();
                        hv_I.Dispose();
                        hv_Exists.Dispose();
                        hv_InputKeys.Dispose();
                        hv_Key.Dispose();
                        hv_Value.Dispose();
                        hv_Indices.Dispose();
                        hv_ValidValues.Dispose();
                        hv_ValidTypes.Dispose();
                        hv_V.Dispose();
                        hv_T.Dispose();
                        hv_IsInt.Dispose();
                        hv_ValidTypesListing.Dispose();
                        hv_ValidValueListing.Dispose();
                        hv_EmptyStrings.Dispose();
                        hv_ImageRangeMinExists.Dispose();
                        hv_ImageRangeMaxExists.Dispose();
                        hv_ImageRangeMin.Dispose();
                        hv_ImageRangeMax.Dispose();
                        hv_IndexParam.Dispose();
                        hv_SetBackgroundID.Dispose();
                        hv_ClassIDsBackground.Dispose();
                        hv_Intersection.Dispose();
                        hv_IgnoreClassIDs.Dispose();
                        hv_KnownClasses.Dispose();
                        hv_IgnoreClassID.Dispose();
                        hv_OptionalKeysExist.Dispose();
                        hv_InstanceType.Dispose();
                        hv_IsInstanceSegmentation.Dispose();
                        hv_IgnoreDirection.Dispose();
                        hv_ClassIDsNoOrientation.Dispose();
                        hv_SemTypes.Dispose();

                        return;
                    }
                    //Set expected values and types.
                    hv_ValidValues.Dispose();
                    hv_ValidValues = new HTuple();
                    hv_ValidTypes.Dispose();
                    hv_ValidTypes = new HTuple();
                    if ((int)(new HTuple(hv_Key.TupleEqual("normalization_type"))) != 0)
                    {
                        hv_ValidValues.Dispose();
                        hv_ValidValues = new HTuple();
                        hv_ValidValues[0] = "all_channels";
                        hv_ValidValues[1] = "first_channel";
                        hv_ValidValues[2] = "constant_values";
                        hv_ValidValues[3] = "none";
                    }
                    else if ((int)(new HTuple(hv_Key.TupleEqual("domain_handling"))) != 0)
                    {
                        if ((int)(new HTuple(hv_DLModelType.TupleEqual("anomaly_detection"))) != 0)
                        {
                            hv_ValidValues.Dispose();
                            hv_ValidValues = new HTuple();
                            hv_ValidValues[0] = "full_domain";
                            hv_ValidValues[1] = "crop_domain";
                            hv_ValidValues[2] = "keep_domain";
                        }
                        else if ((int)(new HTuple(hv_DLModelType.TupleEqual("3d_gripping_point_detection"))) != 0)
                        {
                            hv_ValidValues.Dispose();
                            hv_ValidValues = new HTuple();
                            hv_ValidValues[0] = "full_domain";
                            hv_ValidValues[1] = "crop_domain";
                            hv_ValidValues[2] = "keep_domain";
                        }
                        else
                        {
                            hv_ValidValues.Dispose();
                            hv_ValidValues = new HTuple();
                            hv_ValidValues[0] = "full_domain";
                            hv_ValidValues[1] = "crop_domain";
                        }
                    }
                    else if ((int)(new HTuple(hv_Key.TupleEqual("model_type"))) != 0)
                    {
                        hv_ValidValues.Dispose();
                        hv_ValidValues = new HTuple();
                        hv_ValidValues[0] = "3d_gripping_point_detection";
                        hv_ValidValues[1] = "anomaly_detection";
                        hv_ValidValues[2] = "classification";
                        hv_ValidValues[3] = "detection";
                        hv_ValidValues[4] = "gc_anomaly_detection";
                        hv_ValidValues[5] = "ocr_recognition";
                        hv_ValidValues[6] = "segmentation";
                    }
                    else if ((int)(new HTuple(hv_Key.TupleEqual("augmentation"))) != 0)
                    {
                        hv_ValidValues.Dispose();
                        hv_ValidValues = new HTuple();
                        hv_ValidValues[0] = "true";
                        hv_ValidValues[1] = "false";
                    }
                    else if ((int)(new HTuple(hv_Key.TupleEqual("set_background_id"))) != 0)
                    {
                        hv_ValidTypes.Dispose();
                        hv_ValidTypes = "int";
                    }
                    else if ((int)(new HTuple(hv_Key.TupleEqual("class_ids_background"))) != 0)
                    {
                        hv_ValidTypes.Dispose();
                        hv_ValidTypes = "int";
                    }
                    //Check that type is valid.
                    if ((int)(new HTuple((new HTuple(hv_ValidTypes.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        for (hv_V = 0; (int)hv_V <= (int)((new HTuple(hv_ValidTypes.TupleLength())) - 1); hv_V = (int)hv_V + 1)
                        {
                            hv_T.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_T = hv_ValidTypes.TupleSelect(
                                    hv_V);
                            }
                            if ((int)(new HTuple(hv_T.TupleEqual("int"))) != 0)
                            {
                                hv_IsInt.Dispose();
                                HOperatorSet.TupleIsInt(hv_Value, out hv_IsInt);
                                if ((int)(hv_IsInt.TupleNot()) != 0)
                                {
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        {
                                            HTuple
                                              ExpTmpLocalVar_ValidTypes = ("'" + hv_ValidTypes) + "'";
                                            hv_ValidTypes.Dispose();
                                            hv_ValidTypes = ExpTmpLocalVar_ValidTypes;
                                        }
                                    }
                                    if ((int)(new HTuple((new HTuple(hv_ValidTypes.TupleLength())).TupleLess(
                                        2))) != 0)
                                    {
                                        hv_ValidTypesListing.Dispose();
                                        hv_ValidTypesListing = new HTuple(hv_ValidTypes);
                                    }
                                    else
                                    {
                                        hv_ValidTypesListing.Dispose();
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            hv_ValidTypesListing = ((((hv_ValidTypes.TupleSelectRange(
                                                0, (new HTuple(0)).TupleMax2((new HTuple(hv_ValidTypes.TupleLength()
                                                )) - 2))) + new HTuple(", ")) + (hv_ValidTypes.TupleSelect((new HTuple(hv_ValidTypes.TupleLength()
                                                )) - 1)))).TupleSum();
                                        }
                                    }
                                    throw new HalconException(((((("The value given in the key '" + hv_Key) + "' of DLPreprocessParam is invalid. Valid types are: ") + hv_ValidTypesListing) + ". The given value was '") + hv_Value) + "'.");

                                    hv_CheckParams.Dispose();
                                    hv_KeyExists.Dispose();
                                    hv_DLModelType.Dispose();
                                    hv_Exception.Dispose();
                                    hv_SupportedModelTypes.Dispose();
                                    hv_Index.Dispose();
                                    hv_ParamNamesGeneral.Dispose();
                                    hv_ParamNamesSegmentation.Dispose();
                                    hv_ParamNamesDetectionOptional.Dispose();
                                    hv_ParamNamesPreprocessingOptional.Dispose();
                                    hv_ParamNames3DGrippingPointsOptional.Dispose();
                                    hv_ParamNamesAll.Dispose();
                                    hv_ParamNames.Dispose();
                                    hv_KeysExists.Dispose();
                                    hv_I.Dispose();
                                    hv_Exists.Dispose();
                                    hv_InputKeys.Dispose();
                                    hv_Key.Dispose();
                                    hv_Value.Dispose();
                                    hv_Indices.Dispose();
                                    hv_ValidValues.Dispose();
                                    hv_ValidTypes.Dispose();
                                    hv_V.Dispose();
                                    hv_T.Dispose();
                                    hv_IsInt.Dispose();
                                    hv_ValidTypesListing.Dispose();
                                    hv_ValidValueListing.Dispose();
                                    hv_EmptyStrings.Dispose();
                                    hv_ImageRangeMinExists.Dispose();
                                    hv_ImageRangeMaxExists.Dispose();
                                    hv_ImageRangeMin.Dispose();
                                    hv_ImageRangeMax.Dispose();
                                    hv_IndexParam.Dispose();
                                    hv_SetBackgroundID.Dispose();
                                    hv_ClassIDsBackground.Dispose();
                                    hv_Intersection.Dispose();
                                    hv_IgnoreClassIDs.Dispose();
                                    hv_KnownClasses.Dispose();
                                    hv_IgnoreClassID.Dispose();
                                    hv_OptionalKeysExist.Dispose();
                                    hv_InstanceType.Dispose();
                                    hv_IsInstanceSegmentation.Dispose();
                                    hv_IgnoreDirection.Dispose();
                                    hv_ClassIDsNoOrientation.Dispose();
                                    hv_SemTypes.Dispose();

                                    return;
                                }
                            }
                            else
                            {
                                throw new HalconException("Internal error. Unknown valid type.");
                            }
                        }
                    }
                    //Check that value is valid.
                    if ((int)(new HTuple((new HTuple(hv_ValidValues.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        hv_Index.Dispose();
                        HOperatorSet.TupleFindFirst(hv_ValidValues, hv_Value, out hv_Index);
                        if ((int)(new HTuple(hv_Index.TupleEqual(-1))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_ValidValues = ("'" + hv_ValidValues) + "'";
                                    hv_ValidValues.Dispose();
                                    hv_ValidValues = ExpTmpLocalVar_ValidValues;
                                }
                            }
                            if ((int)(new HTuple((new HTuple(hv_ValidValues.TupleLength())).TupleLess(
                                2))) != 0)
                            {
                                hv_ValidValueListing.Dispose();
                                hv_ValidValueListing = new HTuple(hv_ValidValues);
                            }
                            else
                            {
                                hv_EmptyStrings.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_EmptyStrings = HTuple.TupleGenConst(
                                        (new HTuple(hv_ValidValues.TupleLength())) - 2, "");
                                }
                                hv_ValidValueListing.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_ValidValueListing = ((((hv_ValidValues.TupleSelectRange(
                                        0, (new HTuple(0)).TupleMax2((new HTuple(hv_ValidValues.TupleLength()
                                        )) - 2))) + new HTuple(", ")) + (hv_EmptyStrings.TupleConcat(hv_ValidValues.TupleSelect(
                                        (new HTuple(hv_ValidValues.TupleLength())) - 1))))).TupleSum();
                                }
                            }
                            throw new HalconException(((((("The value given in the key '" + hv_Key) + "' of DLPreprocessParam is invalid. Valid values are: ") + hv_ValidValueListing) + ". The given value was '") + hv_Value) + "'.");
                        }
                    }
                }
                //
                //Check the correct setting of ImageRangeMin and ImageRangeMax.
                if ((int)((new HTuple(hv_DLModelType.TupleEqual("classification"))).TupleOr(
                    new HTuple(hv_DLModelType.TupleEqual("detection")))) != 0)
                {
                    //Check ImageRangeMin and ImageRangeMax.
                    hv_ImageRangeMinExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "image_range_min",
                        out hv_ImageRangeMinExists);
                    hv_ImageRangeMaxExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "image_range_max",
                        out hv_ImageRangeMaxExists);
                    //If they are present, check that they are set correctly.
                    if ((int)(hv_ImageRangeMinExists) != 0)
                    {
                        hv_ImageRangeMin.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_min", out hv_ImageRangeMin);
                        if ((int)(new HTuple(hv_ImageRangeMin.TupleNotEqual(-127))) != 0)
                        {
                            throw new HalconException(("For model type " + hv_DLModelType) + " ImageRangeMin has to be -127.");
                        }
                    }
                    if ((int)(hv_ImageRangeMaxExists) != 0)
                    {
                        hv_ImageRangeMax.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_max", out hv_ImageRangeMax);
                        if ((int)(new HTuple(hv_ImageRangeMax.TupleNotEqual(128))) != 0)
                        {
                            throw new HalconException(("For model type " + hv_DLModelType) + " ImageRangeMax has to be 128.");
                        }
                    }
                }
                //
                //Check segmentation specific parameters.
                if ((int)(new HTuple(hv_DLModelType.TupleEqual("segmentation"))) != 0)
                {
                    //Check if detection specific parameters are set.
                    hv_KeysExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", hv_ParamNamesDetectionOptional,
                        out hv_KeysExists);
                    //If they are present, check that they are [].
                    for (hv_IndexParam = 0; (int)hv_IndexParam <= (int)((new HTuple(hv_ParamNamesDetectionOptional.TupleLength()
                        )) - 1); hv_IndexParam = (int)hv_IndexParam + 1)
                    {
                        if ((int)(hv_KeysExists.TupleSelect(hv_IndexParam)) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Value.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesDetectionOptional.TupleSelect(
                                    hv_IndexParam), out hv_Value);
                            }
                            if ((int)(new HTuple(hv_Value.TupleNotEqual(new HTuple()))) != 0)
                            {
                                throw new HalconException(((("The preprocessing parameter '" + (hv_ParamNamesDetectionOptional.TupleSelect(
                                    hv_IndexParam))) + "' was set to ") + hv_Value) + new HTuple(" but for segmentation it should be set to [], as it is not used for this method."));
                            }
                        }
                    }
                    //Check 'set_background_id'.
                    hv_SetBackgroundID.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "set_background_id", out hv_SetBackgroundID);
                    if ((int)(new HTuple((new HTuple(hv_SetBackgroundID.TupleLength())).TupleGreater(
                        1))) != 0)
                    {
                        throw new HalconException("Only one class_id as 'set_background_id' allowed.");
                    }
                    //Check 'class_ids_background'.
                    hv_ClassIDsBackground.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "class_ids_background", out hv_ClassIDsBackground);
                    if ((int)((new HTuple((new HTuple((new HTuple(hv_SetBackgroundID.TupleLength()
                        )).TupleGreater(0))).TupleAnd((new HTuple((new HTuple(hv_ClassIDsBackground.TupleLength()
                        )).TupleGreater(0))).TupleNot()))).TupleOr((new HTuple((new HTuple(hv_ClassIDsBackground.TupleLength()
                        )).TupleGreater(0))).TupleAnd((new HTuple((new HTuple(hv_SetBackgroundID.TupleLength()
                        )).TupleGreater(0))).TupleNot()))) != 0)
                    {
                        throw new HalconException("Both keys 'set_background_id' and 'class_ids_background' are required.");
                    }
                    //Check that 'class_ids_background' and 'set_background_id' are disjoint.
                    if ((int)(new HTuple((new HTuple(hv_SetBackgroundID.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        hv_Intersection.Dispose();
                        HOperatorSet.TupleIntersection(hv_SetBackgroundID, hv_ClassIDsBackground,
                            out hv_Intersection);
                        if ((int)(new HTuple(hv_Intersection.TupleLength())) != 0)
                        {
                            throw new HalconException("Class IDs in 'set_background_id' and 'class_ids_background' need to be disjoint.");
                        }
                    }
                    //Check 'ignore_class_ids'.
                    hv_IgnoreClassIDs.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "ignore_class_ids", out hv_IgnoreClassIDs);
                    hv_KnownClasses.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_KnownClasses = new HTuple();
                        hv_KnownClasses = hv_KnownClasses.TupleConcat(hv_SetBackgroundID, hv_ClassIDsBackground);
                    }
                    for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_IgnoreClassIDs.TupleLength()
                        )) - 1); hv_I = (int)hv_I + 1)
                    {
                        hv_IgnoreClassID.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IgnoreClassID = hv_IgnoreClassIDs.TupleSelect(
                                hv_I);
                        }
                        hv_Index.Dispose();
                        HOperatorSet.TupleFindFirst(hv_KnownClasses, hv_IgnoreClassID, out hv_Index);
                        if ((int)((new HTuple((new HTuple(hv_Index.TupleLength())).TupleGreater(
                            0))).TupleAnd(new HTuple(hv_Index.TupleNotEqual(-1)))) != 0)
                        {
                            throw new HalconException("The given 'ignore_class_ids' must not be included in the 'class_ids_background' or 'set_background_id'.");
                        }
                    }
                }
                else if ((int)(new HTuple(hv_DLModelType.TupleEqual("detection"))) != 0)
                {
                    //Check if segmentation specific parameters are set.
                    hv_KeysExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", hv_ParamNamesSegmentation,
                        out hv_KeysExists);
                    //If they are present, check that they are [].
                    for (hv_IndexParam = 0; (int)hv_IndexParam <= (int)((new HTuple(hv_ParamNamesSegmentation.TupleLength()
                        )) - 1); hv_IndexParam = (int)hv_IndexParam + 1)
                    {
                        if ((int)(hv_KeysExists.TupleSelect(hv_IndexParam)) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Value.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesSegmentation.TupleSelect(
                                    hv_IndexParam), out hv_Value);
                            }
                            if ((int)(new HTuple(hv_Value.TupleNotEqual(new HTuple()))) != 0)
                            {
                                throw new HalconException(((("The preprocessing parameter '" + (hv_ParamNamesSegmentation.TupleSelect(
                                    hv_IndexParam))) + "' was set to ") + hv_Value) + new HTuple(" but for detection it should be set to [], as it is not used for this method."));
                            }
                        }
                    }
                    //Check optional parameters.
                    hv_OptionalKeysExist.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", hv_ParamNamesDetectionOptional,
                        out hv_OptionalKeysExist);
                    if ((int)(hv_OptionalKeysExist.TupleSelect(0)) != 0)
                    {
                        //Check 'instance_type'.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_InstanceType.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesDetectionOptional.TupleSelect(
                                0), out hv_InstanceType);
                        }
                        if ((int)(new HTuple((new HTuple((((new HTuple("rectangle1")).TupleConcat(
                            "rectangle2")).TupleConcat("mask")).TupleFind(hv_InstanceType))).TupleEqual(
                            -1))) != 0)
                        {
                            throw new HalconException(("Invalid generic parameter for 'instance_type': " + hv_InstanceType) + new HTuple(", only 'rectangle1' and 'rectangle2' are allowed"));
                        }
                    }
                    //If instance_segmentation is set we might overwrite the instance_type for the preprocessing.
                    if ((int)(hv_OptionalKeysExist.TupleSelect(3)) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IsInstanceSegmentation.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesDetectionOptional.TupleSelect(
                                3), out hv_IsInstanceSegmentation);
                        }
                        if ((int)(new HTuple((new HTuple(((((new HTuple(1)).TupleConcat(0)).TupleConcat(
                            "true")).TupleConcat("false")).TupleFind(hv_IsInstanceSegmentation))).TupleEqual(
                            -1))) != 0)
                        {
                            throw new HalconException(("Invalid generic parameter for 'instance_segmentation': " + hv_IsInstanceSegmentation) + new HTuple(", only true, false, 'true' and 'false' are allowed"));
                        }
                    }
                    if ((int)(hv_OptionalKeysExist.TupleSelect(1)) != 0)
                    {
                        //Check 'ignore_direction'.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IgnoreDirection.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesDetectionOptional.TupleSelect(
                                1), out hv_IgnoreDirection);
                        }
                        if ((int)(new HTuple((new HTuple(((new HTuple(1)).TupleConcat(0)).TupleFind(
                            hv_IgnoreDirection))).TupleEqual(-1))) != 0)
                        {
                            throw new HalconException(("Invalid generic parameter for 'ignore_direction': " + hv_IgnoreDirection) + new HTuple(", only true and false are allowed"));
                        }
                    }
                    if ((int)(hv_OptionalKeysExist.TupleSelect(2)) != 0)
                    {
                        //Check 'class_ids_no_orientation'.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ClassIDsNoOrientation.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesDetectionOptional.TupleSelect(
                                2), out hv_ClassIDsNoOrientation);
                        }
                        hv_SemTypes.Dispose();
                        HOperatorSet.TupleSemTypeElem(hv_ClassIDsNoOrientation, out hv_SemTypes);
                        if ((int)((new HTuple(hv_ClassIDsNoOrientation.TupleNotEqual(new HTuple()))).TupleAnd(
                            new HTuple(((((hv_SemTypes.TupleEqualElem("integer"))).TupleSum())).TupleNotEqual(
                            new HTuple(hv_ClassIDsNoOrientation.TupleLength()))))) != 0)
                        {
                            throw new HalconException(("Invalid generic parameter for 'class_ids_no_orientation': " + hv_ClassIDsNoOrientation) + new HTuple(", only integers are allowed"));
                        }
                        else
                        {
                            if ((int)((new HTuple(hv_ClassIDsNoOrientation.TupleNotEqual(new HTuple()))).TupleAnd(
                                new HTuple(((((hv_ClassIDsNoOrientation.TupleGreaterEqualElem(0))).TupleSum()
                                )).TupleNotEqual(new HTuple(hv_ClassIDsNoOrientation.TupleLength()
                                ))))) != 0)
                            {
                                throw new HalconException(("Invalid generic parameter for 'class_ids_no_orientation': " + hv_ClassIDsNoOrientation) + new HTuple(", only non-negative integers are allowed"));
                            }
                        }
                    }
                }
                //

                hv_CheckParams.Dispose();
                hv_KeyExists.Dispose();
                hv_DLModelType.Dispose();
                hv_Exception.Dispose();
                hv_SupportedModelTypes.Dispose();
                hv_Index.Dispose();
                hv_ParamNamesGeneral.Dispose();
                hv_ParamNamesSegmentation.Dispose();
                hv_ParamNamesDetectionOptional.Dispose();
                hv_ParamNamesPreprocessingOptional.Dispose();
                hv_ParamNames3DGrippingPointsOptional.Dispose();
                hv_ParamNamesAll.Dispose();
                hv_ParamNames.Dispose();
                hv_KeysExists.Dispose();
                hv_I.Dispose();
                hv_Exists.Dispose();
                hv_InputKeys.Dispose();
                hv_Key.Dispose();
                hv_Value.Dispose();
                hv_Indices.Dispose();
                hv_ValidValues.Dispose();
                hv_ValidTypes.Dispose();
                hv_V.Dispose();
                hv_T.Dispose();
                hv_IsInt.Dispose();
                hv_ValidTypesListing.Dispose();
                hv_ValidValueListing.Dispose();
                hv_EmptyStrings.Dispose();
                hv_ImageRangeMinExists.Dispose();
                hv_ImageRangeMaxExists.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_IndexParam.Dispose();
                hv_SetBackgroundID.Dispose();
                hv_ClassIDsBackground.Dispose();
                hv_Intersection.Dispose();
                hv_IgnoreClassIDs.Dispose();
                hv_KnownClasses.Dispose();
                hv_IgnoreClassID.Dispose();
                hv_OptionalKeysExist.Dispose();
                hv_InstanceType.Dispose();
                hv_IsInstanceSegmentation.Dispose();
                hv_IgnoreDirection.Dispose();
                hv_ClassIDsNoOrientation.Dispose();
                hv_SemTypes.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_CheckParams.Dispose();
                hv_KeyExists.Dispose();
                hv_DLModelType.Dispose();
                hv_Exception.Dispose();
                hv_SupportedModelTypes.Dispose();
                hv_Index.Dispose();
                hv_ParamNamesGeneral.Dispose();
                hv_ParamNamesSegmentation.Dispose();
                hv_ParamNamesDetectionOptional.Dispose();
                hv_ParamNamesPreprocessingOptional.Dispose();
                hv_ParamNames3DGrippingPointsOptional.Dispose();
                hv_ParamNamesAll.Dispose();
                hv_ParamNames.Dispose();
                hv_KeysExists.Dispose();
                hv_I.Dispose();
                hv_Exists.Dispose();
                hv_InputKeys.Dispose();
                hv_Key.Dispose();
                hv_Value.Dispose();
                hv_Indices.Dispose();
                hv_ValidValues.Dispose();
                hv_ValidTypes.Dispose();
                hv_V.Dispose();
                hv_T.Dispose();
                hv_IsInt.Dispose();
                hv_ValidTypesListing.Dispose();
                hv_ValidValueListing.Dispose();
                hv_EmptyStrings.Dispose();
                hv_ImageRangeMinExists.Dispose();
                hv_ImageRangeMaxExists.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_IndexParam.Dispose();
                hv_SetBackgroundID.Dispose();
                hv_ClassIDsBackground.Dispose();
                hv_Intersection.Dispose();
                hv_IgnoreClassIDs.Dispose();
                hv_KnownClasses.Dispose();
                hv_IgnoreClassID.Dispose();
                hv_OptionalKeysExist.Dispose();
                hv_InstanceType.Dispose();
                hv_IsInstanceSegmentation.Dispose();
                hv_IgnoreDirection.Dispose();
                hv_ClassIDsNoOrientation.Dispose();
                hv_SemTypes.Dispose();

                throw HDevExpDefaultException;
            }
        }



        // Procedures 
        // Chapter: Deep Learning / Model
        // Short Description: Store the given images in a tuple of dictionaries DLSamples. 
        static public void gen_dl_samples_from_images(HObject ho_Images, out HTuple hv_DLSampleBatch)
        {



            // Local iconic variables 

            HObject ho_Image = null;

            // Local control variables 

            HTuple hv_NumImages = new HTuple(), hv_ImageIndex = new HTuple();
            HTuple hv_DLSample = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            hv_DLSampleBatch = new HTuple();
            //
            //This procedure creates DLSampleBatch, a tuple
            //containing a dictionary DLSample
            //for every image given in Images.
            //
            //Initialize output tuple.
            hv_NumImages.Dispose();
            HOperatorSet.CountObj(ho_Images, out hv_NumImages);
            hv_DLSampleBatch.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_DLSampleBatch = HTuple.TupleGenConst(
                    hv_NumImages, -1);
            }
            //
            //Loop through all given images.
            HTuple end_val10 = hv_NumImages - 1;
            HTuple step_val10 = 1;
            for (hv_ImageIndex = 0; hv_ImageIndex.Continue(end_val10, step_val10); hv_ImageIndex = hv_ImageIndex.TupleAdd(step_val10))
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_Image.Dispose();
                    HOperatorSet.SelectObj(ho_Images, out ho_Image, hv_ImageIndex + 1);
                }
                //Create DLSample from image.
                hv_DLSample.Dispose();
                HOperatorSet.CreateDict(out hv_DLSample);
                HOperatorSet.SetDictObject(ho_Image, hv_DLSample, "image");
                //
                //Collect the DLSamples.
                if (hv_DLSampleBatch == null)
                    hv_DLSampleBatch = new HTuple();
                hv_DLSampleBatch[hv_ImageIndex] = hv_DLSample;
            }
            ho_Image.Dispose();

            hv_NumImages.Dispose();
            hv_ImageIndex.Dispose();
            hv_DLSample.Dispose();

            return;
        }



        static public void preprocess_dl_samples(HTuple hv_DLSampleBatch, HTuple hv_DLPreprocessParam)
        {



            // Local iconic variables 

            HObject ho_ImageRaw = null, ho_ImagePreprocessed = null;
            HObject ho_AnomalyImageRaw = null, ho_AnomalyImagePreprocessed = null;
            HObject ho_SegmentationRaw = null, ho_SegmentationPreprocessed = null;
            HObject ho_ImageRawDomain = null;

            // Local control variables 

            HTuple hv_SampleIndex = new HTuple(), hv_DLSample = new HTuple();
            HTuple hv_ImageExists = new HTuple(), hv_KeysExists = new HTuple();
            HTuple hv_AnomalyParamExist = new HTuple(), hv_Rectangle1ParamExist = new HTuple();
            HTuple hv_Rectangle2ParamExist = new HTuple(), hv_InstanceMaskParamExist = new HTuple();
            HTuple hv_SegmentationParamExist = new HTuple();
            HTuple hv_DLPreprocessParam_COPY_INP_TMP = new HTuple(hv_DLPreprocessParam);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageRaw);
            HOperatorSet.GenEmptyObj(out ho_ImagePreprocessed);
            HOperatorSet.GenEmptyObj(out ho_AnomalyImageRaw);
            HOperatorSet.GenEmptyObj(out ho_AnomalyImagePreprocessed);
            HOperatorSet.GenEmptyObj(out ho_SegmentationRaw);
            HOperatorSet.GenEmptyObj(out ho_SegmentationPreprocessed);
            HOperatorSet.GenEmptyObj(out ho_ImageRawDomain);
            try
            {
                //
                //This procedure preprocesses all images of the sample dictionaries
                //in the tuple DLSampleBatch.
                //The images are preprocessed according to the parameters provided
                //in DLPreprocessParam.
                //
                //Check the validity of the preprocessing parameters.
                //The procedure check_dl_preprocess_param might change DLPreprocessParam.
                //To avoid race conditions when preprocess_dl_samples is used from
                //multiple threads with the same DLPreprocessParam dictionary,
                //work on a copy.
                {
                    HTuple ExpTmpOutVar_0;
                    HOperatorSet.CopyDict(hv_DLPreprocessParam_COPY_INP_TMP, new HTuple(), new HTuple(),
                        out ExpTmpOutVar_0);
                    hv_DLPreprocessParam_COPY_INP_TMP.Dispose();
                    hv_DLPreprocessParam_COPY_INP_TMP = ExpTmpOutVar_0;
                }
                check_dl_preprocess_param(hv_DLPreprocessParam_COPY_INP_TMP);
                //
                //
                //
                //Preprocess the sample entries.
                //
                for (hv_SampleIndex = 0; (int)hv_SampleIndex <= (int)((new HTuple(hv_DLSampleBatch.TupleLength()
                    )) - 1); hv_SampleIndex = (int)hv_SampleIndex + 1)
                {
                    hv_DLSample.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DLSample = hv_DLSampleBatch.TupleSelect(
                            hv_SampleIndex);
                    }
                    //
                    //Preprocess augmentation data.
                    preprocess_dl_model_augmentation_data(hv_DLSample, hv_DLPreprocessParam_COPY_INP_TMP);
                    //
                    //Check the existence of the sample keys.
                    hv_ImageExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLSample, "key_exists", "image", out hv_ImageExists);
                    //
                    //Preprocess the images.
                    if ((int)(hv_ImageExists) != 0)
                    {
                        //
                        //Get the image.
                        ho_ImageRaw.Dispose();
                        HOperatorSet.GetDictObject(out ho_ImageRaw, hv_DLSample, "image");
                        //
                        //Preprocess the image.
                        ho_ImagePreprocessed.Dispose();
                        preprocess_dl_model_images(ho_ImageRaw, out ho_ImagePreprocessed, hv_DLPreprocessParam_COPY_INP_TMP);
                        //
                        //Replace the image in the dictionary.
                        HOperatorSet.SetDictObject(ho_ImagePreprocessed, hv_DLSample, "image");
                        //
                        //Check existence of model specific sample keys:
                        //- 'anomaly_ground_truth':
                        //  For model 'type' = 'anomaly_detection' and
                        //  model 'type' = 'gc_anomaly_detection'
                        //- 'bbox_row1':
                        //  For 'instance_type' = 'rectangle1' and
                        //  model 'type' = 'detection'
                        //- 'bbox_phi':
                        //  For 'instance_type' = 'rectangle2' and
                        //  model 'type' = 'detection'
                        //- 'mask':
                        //  For 'instance_type' = 'rectangle1',
                        //  model 'type' = 'detection', and
                        //  'instance_segmentation' = true
                        //- 'segmentation_image':
                        //  For model 'type' = 'segmentation'
                        hv_KeysExists.Dispose();
                        HOperatorSet.GetDictParam(hv_DLSample, "key_exists", ((((new HTuple("anomaly_ground_truth")).TupleConcat(
                            "bbox_row1")).TupleConcat("bbox_phi")).TupleConcat("mask")).TupleConcat(
                            "segmentation_image"), out hv_KeysExists);
                        hv_AnomalyParamExist.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_AnomalyParamExist = hv_KeysExists.TupleSelect(
                                0);
                        }
                        hv_Rectangle1ParamExist.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Rectangle1ParamExist = hv_KeysExists.TupleSelect(
                                1);
                        }
                        hv_Rectangle2ParamExist.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Rectangle2ParamExist = hv_KeysExists.TupleSelect(
                                2);
                        }
                        hv_InstanceMaskParamExist.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_InstanceMaskParamExist = hv_KeysExists.TupleSelect(
                                3);
                        }
                        hv_SegmentationParamExist.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_SegmentationParamExist = hv_KeysExists.TupleSelect(
                                4);
                        }
                        //
                        //Preprocess the anomaly ground truth for
                        //model 'type' = 'anomaly_detection' or
                        //model 'type' = 'gc_anomaly_detection' if present.
                        if ((int)(hv_AnomalyParamExist) != 0)
                        {
                            //
                            //Get the anomaly image.
                            ho_AnomalyImageRaw.Dispose();
                            HOperatorSet.GetDictObject(out ho_AnomalyImageRaw, hv_DLSample, "anomaly_ground_truth");
                            //
                            //Preprocess the anomaly image.
                            ho_AnomalyImagePreprocessed.Dispose();
                            preprocess_dl_model_anomaly(ho_AnomalyImageRaw, out ho_AnomalyImagePreprocessed,
                                hv_DLPreprocessParam_COPY_INP_TMP);
                            //
                            //Set preprocessed anomaly image.
                            HOperatorSet.SetDictObject(ho_AnomalyImagePreprocessed, hv_DLSample,
                                "anomaly_ground_truth");
                        }
                        //
                        //Preprocess depending on the model type.
                        //If bounding boxes are given, rescale them as well.
                        if ((int)(hv_Rectangle1ParamExist) != 0)
                        {
                            //
                            //Preprocess the bounding boxes of type 'rectangle1'.
                            //~~~~~~~~~~~~~~~~~~~~~~
                            //preprocess_dl_model_bbox_rect1(ho_ImageRaw, hv_DLSample, hv_DLPreprocessParam_COPY_INP_TMP);
                        }
                        else if ((int)(hv_Rectangle2ParamExist) != 0)
                        {
                            //
                            //Preprocess the bounding boxes of type 'rectangle2'.
                            //~~~~~~~~~~~~~~~~~~~~~~
                            //preprocess_dl_model_bbox_rect2(ho_ImageRaw, hv_DLSample, hv_DLPreprocessParam_COPY_INP_TMP);
                        }
                        if ((int)(hv_InstanceMaskParamExist) != 0)
                        {
                            //
                            //Preprocess the instance masks.
                            //~~~~~~~~~~~~~~~~~~~~~~
                            //preprocess_dl_model_instance_masks(ho_ImageRaw, hv_DLSample, hv_DLPreprocessParam_COPY_INP_TMP);
                        }
                        //
                        //Preprocess the segmentation image if present.
                        if ((int)(hv_SegmentationParamExist) != 0)
                        {
                            //
                            //Get the segmentation image.
                            ho_SegmentationRaw.Dispose();
                            HOperatorSet.GetDictObject(out ho_SegmentationRaw, hv_DLSample, "segmentation_image");
                            //
                            //Preprocess the segmentation image.
                            ho_SegmentationPreprocessed.Dispose();
                            //~~~~~~~~~~~~~~~~~~~~~~
                            //preprocess_dl_model_segmentations(ho_ImageRaw, ho_SegmentationRaw, out ho_SegmentationPreprocessed,
                            //    hv_DLPreprocessParam_COPY_INP_TMP);


                            //
                            //Set preprocessed segmentation image.
                            HOperatorSet.SetDictObject(ho_SegmentationPreprocessed, hv_DLSample,
                                "segmentation_image");
                        }
                        //
                        //Preprocess 3D relevant data if present.
                        hv_KeysExists.Dispose();
                        HOperatorSet.GetDictParam(hv_DLSample, "key_exists", (((new HTuple("x")).TupleConcat(
                            "y")).TupleConcat("z")).TupleConcat("normals"), out hv_KeysExists);
                        if ((int)(hv_KeysExists.TupleMax()) != 0)
                        {
                            //We need to handle crop_domain before preprocess_dl_model_3d_data
                            //if it is necessary.
                            //Note, we are not cropping the image of DLSample because it has
                            //been done by preprocess_dl_model_images.
                            //Since we always keep the domain of 3D data we do not need to handle
                            //'keep_domain' or 'full_domain'.
                            ho_ImageRawDomain.Dispose();
                            HOperatorSet.GetDomain(ho_ImageRaw, out ho_ImageRawDomain);

                            //~~~~~~~~~~~~~~~~~~~~~~
                            //crop_dl_sample_image(ho_ImageRawDomain, hv_DLSample, "x", hv_DLPreprocessParam_COPY_INP_TMP);
                            //crop_dl_sample_image(ho_ImageRawDomain, hv_DLSample, "y", hv_DLPreprocessParam_COPY_INP_TMP);
                            //crop_dl_sample_image(ho_ImageRawDomain, hv_DLSample, "z", hv_DLPreprocessParam_COPY_INP_TMP);
                            //crop_dl_sample_image(ho_ImageRawDomain, hv_DLSample, "normals", hv_DLPreprocessParam_COPY_INP_TMP);
                            ////
                            //preprocess_dl_model_3d_data(hv_DLSample, hv_DLPreprocessParam_COPY_INP_TMP);
                        }
                    }
                    else
                    {
                        throw new HalconException((new HTuple("All samples processed need to include an image, but the sample with index ") + hv_SampleIndex) + " does not.");
                    }
                }
                //
                ho_ImageRaw.Dispose();
                ho_ImagePreprocessed.Dispose();
                ho_AnomalyImageRaw.Dispose();
                ho_AnomalyImagePreprocessed.Dispose();
                ho_SegmentationRaw.Dispose();
                ho_SegmentationPreprocessed.Dispose();
                ho_ImageRawDomain.Dispose();

                hv_DLPreprocessParam_COPY_INP_TMP.Dispose();
                hv_SampleIndex.Dispose();
                hv_DLSample.Dispose();
                hv_ImageExists.Dispose();
                hv_KeysExists.Dispose();
                hv_AnomalyParamExist.Dispose();
                hv_Rectangle1ParamExist.Dispose();
                hv_Rectangle2ParamExist.Dispose();
                hv_InstanceMaskParamExist.Dispose();
                hv_SegmentationParamExist.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ImageRaw.Dispose();
                ho_ImagePreprocessed.Dispose();
                ho_AnomalyImageRaw.Dispose();
                ho_AnomalyImagePreprocessed.Dispose();
                ho_SegmentationRaw.Dispose();
                ho_SegmentationPreprocessed.Dispose();
                ho_ImageRawDomain.Dispose();

                hv_DLPreprocessParam_COPY_INP_TMP.Dispose();
                hv_SampleIndex.Dispose();
                hv_DLSample.Dispose();
                hv_ImageExists.Dispose();
                hv_KeysExists.Dispose();
                hv_AnomalyParamExist.Dispose();
                hv_Rectangle1ParamExist.Dispose();
                hv_Rectangle2ParamExist.Dispose();
                hv_InstanceMaskParamExist.Dispose();
                hv_SegmentationParamExist.Dispose();

                throw HDevExpDefaultException;
            }
        }


        // Procedures 
        // Chapter: Deep Learning / Model
        // Short Description: Preprocess the provided DLSample image for augmentation purposes. 
        static public void preprocess_dl_model_augmentation_data(HTuple hv_DLSample, HTuple hv_DLPreprocessParam)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_InputImage = null, ho_ImageHighRes = null;

            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_ImageNumChannels = new HTuple(), hv_ModelType = new HTuple();
            HTuple hv_AugmentationKeyExists = new HTuple(), hv_ImageKeyExists = new HTuple();
            HTuple hv_NumImages = new HTuple(), hv_NumChannels = new HTuple();
            HTuple hv_ImageType = new HTuple(), hv_InputImageWidth = new HTuple();
            HTuple hv_InputImageHeight = new HTuple(), hv_InputImageWidthHeightRatio = new HTuple();
            HTuple hv_ZoomHeight = new HTuple(), hv_ZoomWidth = new HTuple();
            HTuple hv_HasPadding = new HTuple(), hv___Tmp_Ctrl_Dict_Init_0 = new HTuple();
            HTuple hv___Tmp_Ctrl_Dict_Init_1 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_InputImage);
            HOperatorSet.GenEmptyObj(out ho_ImageHighRes);
            try
            {
                //This procedure preprocesses the provided DLSample image for augmentation purposes.
                //
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get the required preprocessing parameters.
                hv_ImageWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ImageWidth = hv_DLPreprocessParam.TupleGetDictTuple(
                        "image_width");
                }
                hv_ImageHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ImageHeight = hv_DLPreprocessParam.TupleGetDictTuple(
                        "image_height");
                }
                hv_ImageNumChannels.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ImageNumChannels = hv_DLPreprocessParam.TupleGetDictTuple(
                        "image_num_channels");
                }
                hv_ModelType.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ModelType = hv_DLPreprocessParam.TupleGetDictTuple(
                        "model_type");
                }
                //
                //Determine whether the preprocessing is required or not.
                hv_AugmentationKeyExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "augmentation",
                    out hv_AugmentationKeyExists);
                if ((int)(hv_AugmentationKeyExists.TupleNot()) != 0)
                {
                    ho_InputImage.Dispose();
                    ho_ImageHighRes.Dispose();

                    hv_ImageWidth.Dispose();
                    hv_ImageHeight.Dispose();
                    hv_ImageNumChannels.Dispose();
                    hv_ModelType.Dispose();
                    hv_AugmentationKeyExists.Dispose();
                    hv_ImageKeyExists.Dispose();
                    hv_NumImages.Dispose();
                    hv_NumChannels.Dispose();
                    hv_ImageType.Dispose();
                    hv_InputImageWidth.Dispose();
                    hv_InputImageHeight.Dispose();
                    hv_InputImageWidthHeightRatio.Dispose();
                    hv_ZoomHeight.Dispose();
                    hv_ZoomWidth.Dispose();
                    hv_HasPadding.Dispose();
                    hv___Tmp_Ctrl_Dict_Init_0.Dispose();
                    hv___Tmp_Ctrl_Dict_Init_1.Dispose();

                    return;
                }
                hv___Tmp_Ctrl_Dict_Init_0.Dispose();
                HOperatorSet.CreateDict(out hv___Tmp_Ctrl_Dict_Init_0);
                HOperatorSet.SetDictTuple(hv___Tmp_Ctrl_Dict_Init_0, "comp", "true");
                if ((int)(((((hv_DLPreprocessParam.TupleConcat(hv___Tmp_Ctrl_Dict_Init_0))).TupleTestEqualDictItem(
                    (new HTuple("augmentation")).TupleConcat("comp")))).TupleNot()) != 0)
                {
                    hv___Tmp_Ctrl_Dict_Init_0.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv___Tmp_Ctrl_Dict_Init_0 = HTuple.TupleConstant(
                            "HNULL");
                    }
                    ho_InputImage.Dispose();
                    ho_ImageHighRes.Dispose();

                    hv_ImageWidth.Dispose();
                    hv_ImageHeight.Dispose();
                    hv_ImageNumChannels.Dispose();
                    hv_ModelType.Dispose();
                    hv_AugmentationKeyExists.Dispose();
                    hv_ImageKeyExists.Dispose();
                    hv_NumImages.Dispose();
                    hv_NumChannels.Dispose();
                    hv_ImageType.Dispose();
                    hv_InputImageWidth.Dispose();
                    hv_InputImageHeight.Dispose();
                    hv_InputImageWidthHeightRatio.Dispose();
                    hv_ZoomHeight.Dispose();
                    hv_ZoomWidth.Dispose();
                    hv_HasPadding.Dispose();
                    hv___Tmp_Ctrl_Dict_Init_0.Dispose();
                    hv___Tmp_Ctrl_Dict_Init_1.Dispose();

                    return;
                }
                if ((int)(new HTuple(hv_ModelType.TupleNotEqual("ocr_recognition"))) != 0)
                {
                    ho_InputImage.Dispose();
                    ho_ImageHighRes.Dispose();

                    hv_ImageWidth.Dispose();
                    hv_ImageHeight.Dispose();
                    hv_ImageNumChannels.Dispose();
                    hv_ModelType.Dispose();
                    hv_AugmentationKeyExists.Dispose();
                    hv_ImageKeyExists.Dispose();
                    hv_NumImages.Dispose();
                    hv_NumChannels.Dispose();
                    hv_ImageType.Dispose();
                    hv_InputImageWidth.Dispose();
                    hv_InputImageHeight.Dispose();
                    hv_InputImageWidthHeightRatio.Dispose();
                    hv_ZoomHeight.Dispose();
                    hv_ZoomWidth.Dispose();
                    hv_HasPadding.Dispose();
                    hv___Tmp_Ctrl_Dict_Init_0.Dispose();
                    hv___Tmp_Ctrl_Dict_Init_1.Dispose();

                    return;
                }
                //
                //Get the input image and its properties.
                hv_ImageKeyExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLSample, "key_exists", "image", out hv_ImageKeyExists);
                if ((int)(hv_ImageKeyExists.TupleNot()) != 0)
                {
                    throw new HalconException("The sample to process needs to include an image.");
                }
                ho_InputImage.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_InputImage = hv_DLSample.TupleGetDictObject(
                        "image");
                }
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_InputImage, out hv_NumImages);
                if ((int)(new HTuple(hv_NumImages.TupleNotEqual(1))) != 0)
                {
                    throw new HalconException("The sample to process needs to include exactly 1 image.");
                }
                hv_NumChannels.Dispose();
                HOperatorSet.CountChannels(ho_InputImage, out hv_NumChannels);
                hv_ImageType.Dispose();
                HOperatorSet.GetImageType(ho_InputImage, out hv_ImageType);
                hv_InputImageWidth.Dispose(); hv_InputImageHeight.Dispose();
                HOperatorSet.GetImageSize(ho_InputImage, out hv_InputImageWidth, out hv_InputImageHeight);
                //
                //Execute model specific preprocessing.
                if ((int)(new HTuple(hv_ModelType.TupleEqual("ocr_recognition"))) != 0)
                {
                    if ((int)(new HTuple(hv_ImageNumChannels.TupleNotEqual(1))) != 0)
                    {
                        throw new HalconException("The only 'image_num_channels' value supported for ocr_recognition models is 1.");
                    }
                    if ((int)(new HTuple((new HTuple(hv_ImageType.TupleRegexpTest("byte|real"))).TupleNotEqual(
                        1))) != 0)
                    {
                        throw new HalconException("Please provide only images of type 'byte' or 'real' for ocr_recognition models.");
                    }
                    if ((int)(new HTuple((new HTuple((new HTuple(((hv_NumChannels.TupleEqualElem(
                        1))).TupleOr(hv_NumChannels.TupleEqualElem(3)))).TupleSum())).TupleNotEqual(
                        1))) != 0)
                    {
                        throw new HalconException("Please provide only 1- or 3-channels images for ocr_recognition models.");
                    }
                    //
                    ho_ImageHighRes.Dispose();
                    HOperatorSet.FullDomain(ho_InputImage, out ho_ImageHighRes);
                    if ((int)(new HTuple(hv_NumChannels.TupleEqual(3))) != 0)
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.Rgb1ToGray(ho_ImageHighRes, out ExpTmpOutVar_0);
                            ho_ImageHighRes.Dispose();
                            ho_ImageHighRes = ExpTmpOutVar_0;
                        }
                    }
                    hv_InputImageWidthHeightRatio.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_InputImageWidthHeightRatio = hv_InputImageWidth / (hv_InputImageHeight.TupleReal()
                            );
                    }
                    hv_ZoomHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ZoomHeight = hv_InputImageHeight.TupleMin2(
                            2 * hv_ImageHeight);
                    }
                    hv_ZoomWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ZoomWidth = ((hv_ZoomHeight * hv_InputImageWidthHeightRatio)).TupleInt()
                            ;
                    }
                    hv_HasPadding.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_HasPadding = new HTuple(((((hv_ImageHeight * hv_InputImageWidthHeightRatio)).TupleInt()
                            )).TupleLess(hv_ImageWidth));
                    }
                    if ((int)((new HTuple(hv_ZoomHeight.TupleGreater(hv_ImageHeight))).TupleOr(
                        hv_HasPadding)) != 0)
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ZoomImageSize(ho_ImageHighRes, out ExpTmpOutVar_0, hv_ZoomWidth,
                                hv_ZoomHeight, "constant");
                            ho_ImageHighRes.Dispose();
                            ho_ImageHighRes = ExpTmpOutVar_0;
                        }
                        hv___Tmp_Ctrl_Dict_Init_1.Dispose();
                        HOperatorSet.CreateDict(out hv___Tmp_Ctrl_Dict_Init_1);
                        HOperatorSet.SetDictTuple(hv_DLSample, "augmentation_data", hv___Tmp_Ctrl_Dict_Init_1);
                        hv___Tmp_Ctrl_Dict_Init_1.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv___Tmp_Ctrl_Dict_Init_1 = HTuple.TupleConstant(
                                "HNULL");
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictObject(ho_ImageHighRes, hv_DLSample.TupleGetDictTuple(
                                "augmentation_data"), "image_high_res");
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_DLSample.TupleGetDictTuple("augmentation_data"),
                                "preprocess_params", hv_DLPreprocessParam);
                        }
                    }
                }
                //
                ho_InputImage.Dispose();
                ho_ImageHighRes.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ModelType.Dispose();
                hv_AugmentationKeyExists.Dispose();
                hv_ImageKeyExists.Dispose();
                hv_NumImages.Dispose();
                hv_NumChannels.Dispose();
                hv_ImageType.Dispose();
                hv_InputImageWidth.Dispose();
                hv_InputImageHeight.Dispose();
                hv_InputImageWidthHeightRatio.Dispose();
                hv_ZoomHeight.Dispose();
                hv_ZoomWidth.Dispose();
                hv_HasPadding.Dispose();
                hv___Tmp_Ctrl_Dict_Init_0.Dispose();
                hv___Tmp_Ctrl_Dict_Init_1.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_InputImage.Dispose();
                ho_ImageHighRes.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ModelType.Dispose();
                hv_AugmentationKeyExists.Dispose();
                hv_ImageKeyExists.Dispose();
                hv_NumImages.Dispose();
                hv_NumChannels.Dispose();
                hv_ImageType.Dispose();
                hv_InputImageWidth.Dispose();
                hv_InputImageHeight.Dispose();
                hv_InputImageWidthHeightRatio.Dispose();
                hv_ZoomHeight.Dispose();
                hv_ZoomWidth.Dispose();
                hv_HasPadding.Dispose();
                hv___Tmp_Ctrl_Dict_Init_0.Dispose();
                hv___Tmp_Ctrl_Dict_Init_1.Dispose();

                throw HDevExpDefaultException;
            }
        }




        // Procedures 
        // Chapter: Deep Learning / Model
        // Short Description: Preprocess images for deep-learning-based training and inference. 
        static public void preprocess_dl_model_images(HObject ho_Images, out HObject ho_ImagesPreprocessed,
            HTuple hv_DLPreprocessParam)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_PreservedDomains = null, ho_ImageSelected = null;
            HObject ho_DomainSelected = null, ho_ImagesScaled = null, ho_ImageScaled = null;
            HObject ho_Channel = null, ho_ChannelScaled = null, ho_ThreeChannelImage = null;
            HObject ho_SingleChannelImage = null;

            // Local copy input parameter variables 
            HObject ho_Images_COPY_INP_TMP;
            ho_Images_COPY_INP_TMP = new HObject(ho_Images);



            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_ImageNumChannels = new HTuple(), hv_ImageRangeMin = new HTuple();
            HTuple hv_ImageRangeMax = new HTuple(), hv_DomainHandling = new HTuple();
            HTuple hv_NormalizationType = new HTuple(), hv_ModelType = new HTuple();
            HTuple hv_NumImages = new HTuple(), hv_Type = new HTuple();
            HTuple hv_NumMatches = new HTuple(), hv_InputNumChannels = new HTuple();
            HTuple hv_OutputNumChannels = new HTuple(), hv_NumChannels1 = new HTuple();
            HTuple hv_NumChannels3 = new HTuple(), hv_AreInputNumChannels1 = new HTuple();
            HTuple hv_AreInputNumChannels3 = new HTuple(), hv_AreInputNumChannels1Or3 = new HTuple();
            HTuple hv_ValidNumChannels = new HTuple(), hv_ValidNumChannelsText = new HTuple();
            HTuple hv_PreserveDomain = new HTuple(), hv_Row1 = new HTuple();
            HTuple hv_Column1 = new HTuple(), hv_Row2 = new HTuple();
            HTuple hv_Column2 = new HTuple(), hv_UniqRow1 = new HTuple();
            HTuple hv_UniqColumn1 = new HTuple(), hv_UniqRow2 = new HTuple();
            HTuple hv_UniqColumn2 = new HTuple(), hv_RectangleIndex = new HTuple();
            HTuple hv_OriginalWidth = new HTuple(), hv_OriginalHeight = new HTuple();
            HTuple hv_UniqWidth = new HTuple(), hv_UniqHeight = new HTuple();
            HTuple hv_ScaleWidth = new HTuple(), hv_ScaleHeight = new HTuple();
            HTuple hv_ScaleIndex = new HTuple(), hv_ImageIndex = new HTuple();
            HTuple hv_NumChannels = new HTuple(), hv_ChannelIndex = new HTuple();
            HTuple hv_Min = new HTuple(), hv_Max = new HTuple(), hv_Range = new HTuple();
            HTuple hv_Scale = new HTuple(), hv_Shift = new HTuple();
            HTuple hv_MeanValues = new HTuple(), hv_DeviationValues = new HTuple();
            HTuple hv_UseDefaultNormalizationValues = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_Indices = new HTuple();
            HTuple hv_RescaleRange = new HTuple(), hv_CurrentNumChannels = new HTuple();
            HTuple hv_DiffNumChannelsIndices = new HTuple(), hv_Index = new HTuple();
            HTuple hv_DiffNumChannelsIndex = new HTuple(), hv_NumDomains = new HTuple();
            HTuple hv_DomainIndex = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImagesPreprocessed);
            HOperatorSet.GenEmptyObj(out ho_PreservedDomains);
            HOperatorSet.GenEmptyObj(out ho_ImageSelected);
            HOperatorSet.GenEmptyObj(out ho_DomainSelected);
            HOperatorSet.GenEmptyObj(out ho_ImagesScaled);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_Channel);
            HOperatorSet.GenEmptyObj(out ho_ChannelScaled);
            HOperatorSet.GenEmptyObj(out ho_ThreeChannelImage);
            HOperatorSet.GenEmptyObj(out ho_SingleChannelImage);
            try
            {
                //
                //This procedure preprocesses the provided Images according to the parameters in
                //the dictionary DLPreprocessParam. Note that depending on the images, additional
                //preprocessing steps might be beneficial.
                //
                //Validate the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get the preprocessing parameters.
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_ImageNumChannels.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_num_channels", out hv_ImageNumChannels);
                hv_ImageRangeMin.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_min", out hv_ImageRangeMin);
                hv_ImageRangeMax.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_max", out hv_ImageRangeMax);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                hv_NormalizationType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "normalization_type", out hv_NormalizationType);
                hv_ModelType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "model_type", out hv_ModelType);
                //
                //Validate the type of the input images.
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_Images_COPY_INP_TMP, out hv_NumImages);
                if ((int)(new HTuple(hv_NumImages.TupleEqual(0))) != 0)
                {
                    throw new HalconException("Please provide some images to preprocess.");
                }
                hv_Type.Dispose();
                HOperatorSet.GetImageType(ho_Images_COPY_INP_TMP, out hv_Type);
                hv_NumMatches.Dispose();
                HOperatorSet.TupleRegexpTest(hv_Type, "byte|int|real", out hv_NumMatches);
                if ((int)(new HTuple(hv_NumMatches.TupleNotEqual(hv_NumImages))) != 0)
                {
                    throw new HalconException(new HTuple("Please provide only images of type 'byte', 'int1', 'int2', 'uint2', 'int4', 'int8', or 'real'."));
                }
                //
                //Handle ocr_recognition models.
                if ((int)(new HTuple(hv_ModelType.TupleEqual("ocr_recognition"))) != 0)
                {
                    ho_ImagesPreprocessed.Dispose();

                    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                    //preprocess_dl_model_images_ocr_recognition(ho_Images_COPY_INP_TMP, out ho_ImagesPreprocessed,
                      //  hv_DLPreprocessParam);
                    ho_Images_COPY_INP_TMP.Dispose();
                    ho_PreservedDomains.Dispose();
                    ho_ImageSelected.Dispose();
                    ho_DomainSelected.Dispose();
                    ho_ImagesScaled.Dispose();
                    ho_ImageScaled.Dispose();
                    ho_Channel.Dispose();
                    ho_ChannelScaled.Dispose();
                    ho_ThreeChannelImage.Dispose();
                    ho_SingleChannelImage.Dispose();

                    hv_ImageWidth.Dispose();
                    hv_ImageHeight.Dispose();
                    hv_ImageNumChannels.Dispose();
                    hv_ImageRangeMin.Dispose();
                    hv_ImageRangeMax.Dispose();
                    hv_DomainHandling.Dispose();
                    hv_NormalizationType.Dispose();
                    hv_ModelType.Dispose();
                    hv_NumImages.Dispose();
                    hv_Type.Dispose();
                    hv_NumMatches.Dispose();
                    hv_InputNumChannels.Dispose();
                    hv_OutputNumChannels.Dispose();
                    hv_NumChannels1.Dispose();
                    hv_NumChannels3.Dispose();
                    hv_AreInputNumChannels1.Dispose();
                    hv_AreInputNumChannels3.Dispose();
                    hv_AreInputNumChannels1Or3.Dispose();
                    hv_ValidNumChannels.Dispose();
                    hv_ValidNumChannelsText.Dispose();
                    hv_PreserveDomain.Dispose();
                    hv_Row1.Dispose();
                    hv_Column1.Dispose();
                    hv_Row2.Dispose();
                    hv_Column2.Dispose();
                    hv_UniqRow1.Dispose();
                    hv_UniqColumn1.Dispose();
                    hv_UniqRow2.Dispose();
                    hv_UniqColumn2.Dispose();
                    hv_RectangleIndex.Dispose();
                    hv_OriginalWidth.Dispose();
                    hv_OriginalHeight.Dispose();
                    hv_UniqWidth.Dispose();
                    hv_UniqHeight.Dispose();
                    hv_ScaleWidth.Dispose();
                    hv_ScaleHeight.Dispose();
                    hv_ScaleIndex.Dispose();
                    hv_ImageIndex.Dispose();
                    hv_NumChannels.Dispose();
                    hv_ChannelIndex.Dispose();
                    hv_Min.Dispose();
                    hv_Max.Dispose();
                    hv_Range.Dispose();
                    hv_Scale.Dispose();
                    hv_Shift.Dispose();
                    hv_MeanValues.Dispose();
                    hv_DeviationValues.Dispose();
                    hv_UseDefaultNormalizationValues.Dispose();
                    hv_Exception.Dispose();
                    hv_Indices.Dispose();
                    hv_RescaleRange.Dispose();
                    hv_CurrentNumChannels.Dispose();
                    hv_DiffNumChannelsIndices.Dispose();
                    hv_Index.Dispose();
                    hv_DiffNumChannelsIndex.Dispose();
                    hv_NumDomains.Dispose();
                    hv_DomainIndex.Dispose();

                    return;
                }
                //
                //Validate the number channels of the input images.
                hv_InputNumChannels.Dispose();
                HOperatorSet.CountChannels(ho_Images_COPY_INP_TMP, out hv_InputNumChannels);
                hv_OutputNumChannels.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_OutputNumChannels = HTuple.TupleGenConst(
                        hv_NumImages, hv_ImageNumChannels);
                }
                //Only for 'image_num_channels' 1 and 3 combinations of 1- and 3-channel images are allowed.
                if ((int)((new HTuple(hv_ImageNumChannels.TupleEqual(1))).TupleOr(new HTuple(hv_ImageNumChannels.TupleEqual(
                    3)))) != 0)
                {
                    hv_NumChannels1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_NumChannels1 = HTuple.TupleGenConst(
                            hv_NumImages, 1);
                    }
                    hv_NumChannels3.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_NumChannels3 = HTuple.TupleGenConst(
                            hv_NumImages, 3);
                    }
                    hv_AreInputNumChannels1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_AreInputNumChannels1 = hv_InputNumChannels.TupleEqualElem(
                            hv_NumChannels1);
                    }
                    hv_AreInputNumChannels3.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_AreInputNumChannels3 = hv_InputNumChannels.TupleEqualElem(
                            hv_NumChannels3);
                    }
                    hv_AreInputNumChannels1Or3.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_AreInputNumChannels1Or3 = hv_AreInputNumChannels1 + hv_AreInputNumChannels3;
                    }
                    hv_ValidNumChannels.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ValidNumChannels = new HTuple(hv_AreInputNumChannels1Or3.TupleEqual(
                            hv_NumChannels1));
                    }
                    hv_ValidNumChannelsText.Dispose();
                    hv_ValidNumChannelsText = "Valid numbers of channels for the specified model are 1 or 3.";
                }
                else
                {
                    hv_ValidNumChannels.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ValidNumChannels = new HTuple(hv_InputNumChannels.TupleEqual(
                            hv_OutputNumChannels));
                    }
                    hv_ValidNumChannelsText.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ValidNumChannelsText = ("Valid number of channels for the specified model is " + hv_ImageNumChannels) + ".";
                    }
                }
                if ((int)(hv_ValidNumChannels.TupleNot()) != 0)
                {
                    throw new HalconException("Please provide images with a valid number of channels. " + hv_ValidNumChannelsText);
                }
                //Preprocess the images.
                //
                //For models of type '3d_gripping_point_detection', the preprocessing steps need to be performed on full
                //domain images while the domains are preserved and set back into the images after the preprocessing.
                hv_PreserveDomain.Dispose();
                hv_PreserveDomain = 0;
                if ((int)((new HTuple(hv_ModelType.TupleEqual("3d_gripping_point_detection"))).TupleAnd(
                    (new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))).TupleOr(new HTuple(hv_DomainHandling.TupleEqual(
                    "keep_domain"))))) != 0)
                {
                    hv_PreserveDomain.Dispose();
                    hv_PreserveDomain = 1;
                    ho_PreservedDomains.Dispose();
                    HOperatorSet.GetDomain(ho_Images_COPY_INP_TMP, out ho_PreservedDomains);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FullDomain(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0);
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Apply the domain to the images.
                if ((int)(new HTuple(hv_DomainHandling.TupleEqual("full_domain"))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FullDomain(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0);
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else if ((int)(new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))) != 0)
                {
                    if ((int)(hv_PreserveDomain) != 0)
                    {
                        //In case of preserved domain, the crop is performed with the smallest rectangle of the
                        //domain to avoid out of domain pixels being set to 0.
                        hv_Row1.Dispose(); hv_Column1.Dispose(); hv_Row2.Dispose(); hv_Column2.Dispose();
                        HOperatorSet.SmallestRectangle1(ho_PreservedDomains, out hv_Row1, out hv_Column1,
                            out hv_Row2, out hv_Column2);
                        hv_UniqRow1.Dispose();
                        HOperatorSet.TupleUniq(hv_Row1, out hv_UniqRow1);
                        hv_UniqColumn1.Dispose();
                        HOperatorSet.TupleUniq(hv_Column1, out hv_UniqColumn1);
                        hv_UniqRow2.Dispose();
                        HOperatorSet.TupleUniq(hv_Row2, out hv_UniqRow2);
                        hv_UniqColumn2.Dispose();
                        HOperatorSet.TupleUniq(hv_Column2, out hv_UniqColumn2);
                        if ((int)((new HTuple((new HTuple((new HTuple((new HTuple(hv_UniqRow1.TupleLength()
                            )).TupleEqual(1))).TupleAnd(new HTuple((new HTuple(hv_UniqColumn1.TupleLength()
                            )).TupleEqual(1))))).TupleAnd(new HTuple((new HTuple(hv_UniqRow2.TupleLength()
                            )).TupleEqual(1))))).TupleAnd(new HTuple((new HTuple(hv_UniqColumn2.TupleLength()
                            )).TupleEqual(1)))) != 0)
                        {
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.CropRectangle1(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0,
                                    hv_UniqRow1, hv_UniqColumn1, hv_UniqRow2, hv_UniqColumn2);
                                ho_Images_COPY_INP_TMP.Dispose();
                                ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.MoveRegion(ho_PreservedDomains, out ExpTmpOutVar_0, -hv_UniqRow1,
                                    -hv_UniqColumn1);
                                ho_PreservedDomains.Dispose();
                                ho_PreservedDomains = ExpTmpOutVar_0;
                            }
                        }
                        else
                        {
                            for (hv_RectangleIndex = 0; (int)hv_RectangleIndex <= (int)((new HTuple(hv_Row1.TupleLength()
                                )) - 1); hv_RectangleIndex = (int)hv_RectangleIndex + 1)
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    ho_ImageSelected.Dispose();
                                    HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected,
                                        hv_RectangleIndex + 1);
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.CropRectangle1(ho_ImageSelected, out ExpTmpOutVar_0, hv_Row1.TupleSelect(
                                        hv_RectangleIndex), hv_Column1.TupleSelect(hv_RectangleIndex),
                                        hv_Row2.TupleSelect(hv_RectangleIndex), hv_Column2.TupleSelect(
                                        hv_RectangleIndex));
                                    ho_ImageSelected.Dispose();
                                    ho_ImageSelected = ExpTmpOutVar_0;
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ReplaceObj(ho_Images_COPY_INP_TMP, ho_ImageSelected, out ExpTmpOutVar_0,
                                        hv_RectangleIndex + 1);
                                    ho_Images_COPY_INP_TMP.Dispose();
                                    ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    ho_DomainSelected.Dispose();
                                    HOperatorSet.SelectObj(ho_PreservedDomains, out ho_DomainSelected,
                                        hv_RectangleIndex + 1);
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.MoveRegion(ho_DomainSelected, out ExpTmpOutVar_0, -(hv_Row1.TupleSelect(
                                        hv_RectangleIndex)), -(hv_Column1.TupleSelect(hv_RectangleIndex)));
                                    ho_DomainSelected.Dispose();
                                    ho_DomainSelected = ExpTmpOutVar_0;
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ReplaceObj(ho_PreservedDomains, ho_DomainSelected, out ExpTmpOutVar_0,
                                        hv_RectangleIndex + 1);
                                    ho_PreservedDomains.Dispose();
                                    ho_PreservedDomains = ExpTmpOutVar_0;
                                }
                            }
                        }
                    }
                    else
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.CropDomain(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0);
                            ho_Images_COPY_INP_TMP.Dispose();
                            ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                    }
                }
                else if ((int)((new HTuple(hv_DomainHandling.TupleEqual("keep_domain"))).TupleAnd(
                    (new HTuple(hv_ModelType.TupleEqual("anomaly_detection"))).TupleOr(new HTuple(hv_ModelType.TupleEqual(
                    "3d_gripping_point_detection"))))) != 0)
                {
                    //The option 'keep_domain' is only supported for models of 'type' = 'anomaly_detection' or '3d_gripping_point_detection'.
                }
                else
                {
                    throw new HalconException("Unsupported parameter value for 'domain_handling'.");
                }
                //
                //Zoom preserved domains before zooming the images.
                if ((int)(hv_PreserveDomain) != 0)
                {
                    hv_OriginalWidth.Dispose(); hv_OriginalHeight.Dispose();
                    HOperatorSet.GetImageSize(ho_Images_COPY_INP_TMP, out hv_OriginalWidth, out hv_OriginalHeight);
                    hv_UniqWidth.Dispose();
                    HOperatorSet.TupleUniq(hv_OriginalWidth, out hv_UniqWidth);
                    hv_UniqHeight.Dispose();
                    HOperatorSet.TupleUniq(hv_OriginalHeight, out hv_UniqHeight);
                    if ((int)((new HTuple((new HTuple(hv_UniqWidth.TupleLength())).TupleEqual(
                        1))).TupleAnd(new HTuple((new HTuple(hv_UniqHeight.TupleLength())).TupleEqual(
                        1)))) != 0)
                    {
                        hv_ScaleWidth.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ScaleWidth = hv_ImageWidth / (hv_UniqWidth.TupleReal()
                                );
                        }
                        hv_ScaleHeight.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ScaleHeight = hv_ImageHeight / (hv_UniqHeight.TupleReal()
                                );
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ZoomRegion(ho_PreservedDomains, out ExpTmpOutVar_0, hv_ScaleWidth,
                                hv_ScaleHeight);
                            ho_PreservedDomains.Dispose();
                            ho_PreservedDomains = ExpTmpOutVar_0;
                        }
                    }
                    else
                    {
                        hv_ScaleWidth.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ScaleWidth = hv_ImageWidth / (hv_OriginalWidth.TupleReal()
                                );
                        }
                        hv_ScaleHeight.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ScaleHeight = hv_ImageHeight / (hv_OriginalHeight.TupleReal()
                                );
                        }
                        for (hv_ScaleIndex = 0; (int)hv_ScaleIndex <= (int)((new HTuple(hv_ScaleWidth.TupleLength()
                            )) - 1); hv_ScaleIndex = (int)hv_ScaleIndex + 1)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_DomainSelected.Dispose();
                                HOperatorSet.SelectObj(ho_PreservedDomains, out ho_DomainSelected, hv_ScaleIndex + 1);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ZoomRegion(ho_DomainSelected, out ExpTmpOutVar_0, hv_ScaleWidth.TupleSelect(
                                    hv_ScaleIndex), hv_ScaleHeight.TupleSelect(hv_ScaleIndex));
                                ho_DomainSelected.Dispose();
                                ho_DomainSelected = ExpTmpOutVar_0;
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ReplaceObj(ho_PreservedDomains, ho_DomainSelected, out ExpTmpOutVar_0,
                                    hv_ScaleIndex + 1);
                                ho_PreservedDomains.Dispose();
                                ho_PreservedDomains = ExpTmpOutVar_0;
                            }
                        }
                    }
                }
                //
                //Convert the images to real and zoom the images.
                //Zoom first to speed up if all image types are supported by zoom_image_size.
                if ((int)(new HTuple((new HTuple(hv_Type.TupleRegexpTest("int1|int4|int8"))).TupleEqual(
                    0))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ZoomImageSize(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0, hv_ImageWidth,
                            hv_ImageHeight, "constant");
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0,
                            "real");
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0,
                            "real");
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ZoomImageSize(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0, hv_ImageWidth,
                            hv_ImageHeight, "constant");
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                if ((int)(new HTuple(hv_NormalizationType.TupleEqual("all_channels"))) != 0)
                {
                    //Scale for each image the gray values of all channels to ImageRangeMin-ImageRangeMax.
                    ho_ImagesScaled.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_ImagesScaled);
                    HTuple end_val132 = hv_NumImages;
                    HTuple step_val132 = 1;
                    for (hv_ImageIndex = 1; hv_ImageIndex.Continue(end_val132, step_val132); hv_ImageIndex = hv_ImageIndex.TupleAdd(step_val132))
                    {
                        ho_ImageSelected.Dispose();
                        HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected, hv_ImageIndex);
                        hv_NumChannels.Dispose();
                        HOperatorSet.CountChannels(ho_ImageSelected, out hv_NumChannels);
                        ho_ImageScaled.Dispose();
                        HOperatorSet.GenEmptyObj(out ho_ImageScaled);
                        HTuple end_val136 = hv_NumChannels;
                        HTuple step_val136 = 1;
                        for (hv_ChannelIndex = 1; hv_ChannelIndex.Continue(end_val136, step_val136); hv_ChannelIndex = hv_ChannelIndex.TupleAdd(step_val136))
                        {
                            ho_Channel.Dispose();
                            HOperatorSet.AccessChannel(ho_ImageSelected, out ho_Channel, hv_ChannelIndex);
                            hv_Min.Dispose(); hv_Max.Dispose(); hv_Range.Dispose();
                            HOperatorSet.MinMaxGray(ho_Channel, ho_Channel, 0, out hv_Min, out hv_Max,
                                out hv_Range);
                            if ((int)(new HTuple(((hv_Max - hv_Min)).TupleEqual(0))) != 0)
                            {
                                hv_Scale.Dispose();
                                hv_Scale = 1;
                            }
                            else
                            {
                                hv_Scale.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_Scale = (hv_ImageRangeMax - hv_ImageRangeMin) / (hv_Max - hv_Min);
                                }
                            }
                            hv_Shift.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Shift = ((-hv_Scale) * hv_Min) + hv_ImageRangeMin;
                            }
                            ho_ChannelScaled.Dispose();
                            HOperatorSet.ScaleImage(ho_Channel, out ho_ChannelScaled, hv_Scale, hv_Shift);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.AppendChannel(ho_ImageScaled, ho_ChannelScaled, out ExpTmpOutVar_0
                                    );
                                ho_ImageScaled.Dispose();
                                ho_ImageScaled = ExpTmpOutVar_0;
                            }
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ImagesScaled, ho_ImageScaled, out ExpTmpOutVar_0
                                );
                            ho_ImagesScaled.Dispose();
                            ho_ImagesScaled = ExpTmpOutVar_0;
                        }
                    }
                    ho_Images_COPY_INP_TMP.Dispose();
                    ho_Images_COPY_INP_TMP = new HObject(ho_ImagesScaled);
                }
                else if ((int)(new HTuple(hv_NormalizationType.TupleEqual("first_channel"))) != 0)
                {
                    //Scale for each image the gray values of first channel to ImageRangeMin-ImageRangeMax.
                    ho_ImagesScaled.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_ImagesScaled);
                    HTuple end_val154 = hv_NumImages;
                    HTuple step_val154 = 1;
                    for (hv_ImageIndex = 1; hv_ImageIndex.Continue(end_val154, step_val154); hv_ImageIndex = hv_ImageIndex.TupleAdd(step_val154))
                    {
                        ho_ImageSelected.Dispose();
                        HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected, hv_ImageIndex);
                        hv_Min.Dispose(); hv_Max.Dispose(); hv_Range.Dispose();
                        HOperatorSet.MinMaxGray(ho_ImageSelected, ho_ImageSelected, 0, out hv_Min,
                            out hv_Max, out hv_Range);
                        if ((int)(new HTuple(((hv_Max - hv_Min)).TupleEqual(0))) != 0)
                        {
                            hv_Scale.Dispose();
                            hv_Scale = 1;
                        }
                        else
                        {
                            hv_Scale.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Scale = (hv_ImageRangeMax - hv_ImageRangeMin) / (hv_Max - hv_Min);
                            }
                        }
                        hv_Shift.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Shift = ((-hv_Scale) * hv_Min) + hv_ImageRangeMin;
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ScaleImage(ho_ImageSelected, out ExpTmpOutVar_0, hv_Scale,
                                hv_Shift);
                            ho_ImageSelected.Dispose();
                            ho_ImageSelected = ExpTmpOutVar_0;
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ImagesScaled, ho_ImageSelected, out ExpTmpOutVar_0
                                );
                            ho_ImagesScaled.Dispose();
                            ho_ImagesScaled = ExpTmpOutVar_0;
                        }
                    }
                    ho_Images_COPY_INP_TMP.Dispose();
                    ho_Images_COPY_INP_TMP = new HObject(ho_ImagesScaled);
                }
                else if ((int)(new HTuple(hv_NormalizationType.TupleEqual("constant_values"))) != 0)
                {
                    //Scale for each image the gray values of all channels to the corresponding channel DeviationValues[].
                    try
                    {
                        hv_MeanValues.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "mean_values_normalization",
                            out hv_MeanValues);
                        hv_DeviationValues.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "deviation_values_normalization",
                            out hv_DeviationValues);
                        hv_UseDefaultNormalizationValues.Dispose();
                        hv_UseDefaultNormalizationValues = 0;
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        hv_MeanValues.Dispose();
                        hv_MeanValues = new HTuple();
                        hv_MeanValues[0] = 123.675;
                        hv_MeanValues[1] = 116.28;
                        hv_MeanValues[2] = 103.53;
                        hv_DeviationValues.Dispose();
                        hv_DeviationValues = new HTuple();
                        hv_DeviationValues[0] = 58.395;
                        hv_DeviationValues[1] = 57.12;
                        hv_DeviationValues[2] = 57.375;
                        hv_UseDefaultNormalizationValues.Dispose();
                        hv_UseDefaultNormalizationValues = 1;
                    }
                    ho_ImagesScaled.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_ImagesScaled);
                    HTuple end_val179 = hv_NumImages;
                    HTuple step_val179 = 1;
                    for (hv_ImageIndex = 1; hv_ImageIndex.Continue(end_val179, step_val179); hv_ImageIndex = hv_ImageIndex.TupleAdd(step_val179))
                    {
                        ho_ImageSelected.Dispose();
                        HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected, hv_ImageIndex);
                        hv_NumChannels.Dispose();
                        HOperatorSet.CountChannels(ho_ImageSelected, out hv_NumChannels);
                        //Ensure that the number of channels is equal |DeviationValues| and |MeanValues|
                        if ((int)(hv_UseDefaultNormalizationValues) != 0)
                        {
                            if ((int)(new HTuple(hv_NumChannels.TupleEqual(1))) != 0)
                            {
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.Compose3(ho_ImageSelected, ho_ImageSelected, ho_ImageSelected,
                                        out ExpTmpOutVar_0);
                                    ho_ImageSelected.Dispose();
                                    ho_ImageSelected = ExpTmpOutVar_0;
                                }
                                hv_NumChannels.Dispose();
                                HOperatorSet.CountChannels(ho_ImageSelected, out hv_NumChannels);
                            }
                            else if ((int)(new HTuple(hv_NumChannels.TupleNotEqual(
                                3))) != 0)
                            {
                                throw new HalconException("Using default values for normalization type 'constant_values' is allowed only for 1- and 3-channel images.");
                            }
                        }
                        if ((int)((new HTuple((new HTuple(hv_MeanValues.TupleLength())).TupleNotEqual(
                            hv_NumChannels))).TupleOr(new HTuple((new HTuple(hv_DeviationValues.TupleLength()
                            )).TupleNotEqual(hv_NumChannels)))) != 0)
                        {
                            throw new HalconException("The length of mean and deviation values for normalization type 'constant_values' have to be the same size as the number of channels of the image.");
                        }
                        ho_ImageScaled.Dispose();
                        HOperatorSet.GenEmptyObj(out ho_ImageScaled);
                        HTuple end_val195 = hv_NumChannels;
                        HTuple step_val195 = 1;
                        for (hv_ChannelIndex = 1; hv_ChannelIndex.Continue(end_val195, step_val195); hv_ChannelIndex = hv_ChannelIndex.TupleAdd(step_val195))
                        {
                            ho_Channel.Dispose();
                            HOperatorSet.AccessChannel(ho_ImageSelected, out ho_Channel, hv_ChannelIndex);
                            hv_Scale.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Scale = 1.0 / (hv_DeviationValues.TupleSelect(
                                    hv_ChannelIndex - 1));
                            }
                            hv_Shift.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Shift = (-hv_Scale) * (hv_MeanValues.TupleSelect(
                                    hv_ChannelIndex - 1));
                            }
                            ho_ChannelScaled.Dispose();
                            HOperatorSet.ScaleImage(ho_Channel, out ho_ChannelScaled, hv_Scale, hv_Shift);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.AppendChannel(ho_ImageScaled, ho_ChannelScaled, out ExpTmpOutVar_0
                                    );
                                ho_ImageScaled.Dispose();
                                ho_ImageScaled = ExpTmpOutVar_0;
                            }
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ImagesScaled, ho_ImageScaled, out ExpTmpOutVar_0
                                );
                            ho_ImagesScaled.Dispose();
                            ho_ImagesScaled = ExpTmpOutVar_0;
                        }
                    }
                    ho_Images_COPY_INP_TMP.Dispose();
                    ho_Images_COPY_INP_TMP = new HObject(ho_ImagesScaled);
                }
                else if ((int)(new HTuple(hv_NormalizationType.TupleEqual("none"))) != 0)
                {
                    hv_Indices.Dispose();
                    HOperatorSet.TupleFind(hv_Type, "byte", out hv_Indices);
                    if ((int)(new HTuple(hv_Indices.TupleNotEqual(-1))) != 0)
                    {
                        //Shift the gray values from [0-255] to the expected range for byte images.
                        hv_RescaleRange.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_RescaleRange = (hv_ImageRangeMax - hv_ImageRangeMin) / 255.0;
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_ImageSelected.Dispose();
                            HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected, hv_Indices + 1);
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ScaleImage(ho_ImageSelected, out ExpTmpOutVar_0, hv_RescaleRange,
                                hv_ImageRangeMin);
                            ho_ImageSelected.Dispose();
                            ho_ImageSelected = ExpTmpOutVar_0;
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ReplaceObj(ho_Images_COPY_INP_TMP, ho_ImageSelected, out ExpTmpOutVar_0,
                                hv_Indices + 1);
                            ho_Images_COPY_INP_TMP.Dispose();
                            ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                    }
                }
                else if ((int)(new HTuple(hv_NormalizationType.TupleNotEqual("none"))) != 0)
                {
                    throw new HalconException("Unsupported parameter value for 'normalization_type'");
                }
                //
                //Ensure that the number of channels of the resulting images is consistent with the
                //number of channels of the given model. The only exceptions that are adapted below
                //are combinations of 1- and 3-channel images if ImageNumChannels is either 1 or 3.
                if ((int)((new HTuple(hv_ImageNumChannels.TupleEqual(1))).TupleOr(new HTuple(hv_ImageNumChannels.TupleEqual(
                    3)))) != 0)
                {
                    hv_CurrentNumChannels.Dispose();
                    HOperatorSet.CountChannels(ho_Images_COPY_INP_TMP, out hv_CurrentNumChannels);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DiffNumChannelsIndices.Dispose();
                        HOperatorSet.TupleFind(hv_CurrentNumChannels.TupleNotEqualElem(hv_OutputNumChannels),
                            1, out hv_DiffNumChannelsIndices);
                    }
                    if ((int)(new HTuple(hv_DiffNumChannelsIndices.TupleNotEqual(-1))) != 0)
                    {
                        for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_DiffNumChannelsIndices.TupleLength()
                            )) - 1); hv_Index = (int)hv_Index + 1)
                        {
                            hv_DiffNumChannelsIndex.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_DiffNumChannelsIndex = hv_DiffNumChannelsIndices.TupleSelect(
                                    hv_Index);
                            }
                            hv_ImageIndex.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ImageIndex = hv_DiffNumChannelsIndex + 1;
                            }
                            hv_NumChannels.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_NumChannels = hv_CurrentNumChannels.TupleSelect(
                                    hv_ImageIndex - 1);
                            }
                            ho_ImageSelected.Dispose();
                            HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected,
                                hv_ImageIndex);
                            if ((int)((new HTuple(hv_NumChannels.TupleEqual(1))).TupleAnd(new HTuple(hv_ImageNumChannels.TupleEqual(
                                3)))) != 0)
                            {
                                //Conversion from 1- to 3-channel image required
                                ho_ThreeChannelImage.Dispose();
                                HOperatorSet.Compose3(ho_ImageSelected, ho_ImageSelected, ho_ImageSelected,
                                    out ho_ThreeChannelImage);
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ReplaceObj(ho_Images_COPY_INP_TMP, ho_ThreeChannelImage,
                                        out ExpTmpOutVar_0, hv_ImageIndex);
                                    ho_Images_COPY_INP_TMP.Dispose();
                                    ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                                }
                            }
                            else if ((int)((new HTuple(hv_NumChannels.TupleEqual(3))).TupleAnd(
                                new HTuple(hv_ImageNumChannels.TupleEqual(1)))) != 0)
                            {
                                //Conversion from 3- to 1-channel image required
                                ho_SingleChannelImage.Dispose();
                                HOperatorSet.Rgb1ToGray(ho_ImageSelected, out ho_SingleChannelImage
                                    );
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ReplaceObj(ho_Images_COPY_INP_TMP, ho_SingleChannelImage,
                                        out ExpTmpOutVar_0, hv_ImageIndex);
                                    ho_Images_COPY_INP_TMP.Dispose();
                                    ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                                }
                            }
                            else
                            {
                                throw new HalconException(((("Unexpected error adapting the number of channels. The number of channels of the resulting image is " + hv_NumChannels) + new HTuple(", but the number of channels of the model is ")) + hv_ImageNumChannels) + ".");
                            }
                        }
                    }
                }
                //
                //In case the image domains were preserved, they need to be set back into the images.
                if ((int)(hv_PreserveDomain) != 0)
                {
                    hv_NumDomains.Dispose();
                    HOperatorSet.CountObj(ho_PreservedDomains, out hv_NumDomains);
                    HTuple end_val248 = hv_NumDomains;
                    HTuple step_val248 = 1;
                    for (hv_DomainIndex = 1; hv_DomainIndex.Continue(end_val248, step_val248); hv_DomainIndex = hv_DomainIndex.TupleAdd(step_val248))
                    {
                        ho_ImageSelected.Dispose();
                        HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected, hv_DomainIndex);
                        ho_DomainSelected.Dispose();
                        HOperatorSet.SelectObj(ho_PreservedDomains, out ho_DomainSelected, hv_DomainIndex);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ReduceDomain(ho_ImageSelected, ho_DomainSelected, out ExpTmpOutVar_0
                                );
                            ho_ImageSelected.Dispose();
                            ho_ImageSelected = ExpTmpOutVar_0;
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ReplaceObj(ho_Images_COPY_INP_TMP, ho_ImageSelected, out ExpTmpOutVar_0,
                                hv_DomainIndex);
                            ho_Images_COPY_INP_TMP.Dispose();
                            ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                    }
                }
                //
                //Write preprocessed images to output variable.
                ho_ImagesPreprocessed.Dispose();
                ho_ImagesPreprocessed = new HObject(ho_Images_COPY_INP_TMP);
                //
                ho_Images_COPY_INP_TMP.Dispose();
                ho_PreservedDomains.Dispose();
                ho_ImageSelected.Dispose();
                ho_DomainSelected.Dispose();
                ho_ImagesScaled.Dispose();
                ho_ImageScaled.Dispose();
                ho_Channel.Dispose();
                ho_ChannelScaled.Dispose();
                ho_ThreeChannelImage.Dispose();
                ho_SingleChannelImage.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_NormalizationType.Dispose();
                hv_ModelType.Dispose();
                hv_NumImages.Dispose();
                hv_Type.Dispose();
                hv_NumMatches.Dispose();
                hv_InputNumChannels.Dispose();
                hv_OutputNumChannels.Dispose();
                hv_NumChannels1.Dispose();
                hv_NumChannels3.Dispose();
                hv_AreInputNumChannels1.Dispose();
                hv_AreInputNumChannels3.Dispose();
                hv_AreInputNumChannels1Or3.Dispose();
                hv_ValidNumChannels.Dispose();
                hv_ValidNumChannelsText.Dispose();
                hv_PreserveDomain.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv_UniqRow1.Dispose();
                hv_UniqColumn1.Dispose();
                hv_UniqRow2.Dispose();
                hv_UniqColumn2.Dispose();
                hv_RectangleIndex.Dispose();
                hv_OriginalWidth.Dispose();
                hv_OriginalHeight.Dispose();
                hv_UniqWidth.Dispose();
                hv_UniqHeight.Dispose();
                hv_ScaleWidth.Dispose();
                hv_ScaleHeight.Dispose();
                hv_ScaleIndex.Dispose();
                hv_ImageIndex.Dispose();
                hv_NumChannels.Dispose();
                hv_ChannelIndex.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();
                hv_Scale.Dispose();
                hv_Shift.Dispose();
                hv_MeanValues.Dispose();
                hv_DeviationValues.Dispose();
                hv_UseDefaultNormalizationValues.Dispose();
                hv_Exception.Dispose();
                hv_Indices.Dispose();
                hv_RescaleRange.Dispose();
                hv_CurrentNumChannels.Dispose();
                hv_DiffNumChannelsIndices.Dispose();
                hv_Index.Dispose();
                hv_DiffNumChannelsIndex.Dispose();
                hv_NumDomains.Dispose();
                hv_DomainIndex.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Images_COPY_INP_TMP.Dispose();
                ho_PreservedDomains.Dispose();
                ho_ImageSelected.Dispose();
                ho_DomainSelected.Dispose();
                ho_ImagesScaled.Dispose();
                ho_ImageScaled.Dispose();
                ho_Channel.Dispose();
                ho_ChannelScaled.Dispose();
                ho_ThreeChannelImage.Dispose();
                ho_SingleChannelImage.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_NormalizationType.Dispose();
                hv_ModelType.Dispose();
                hv_NumImages.Dispose();
                hv_Type.Dispose();
                hv_NumMatches.Dispose();
                hv_InputNumChannels.Dispose();
                hv_OutputNumChannels.Dispose();
                hv_NumChannels1.Dispose();
                hv_NumChannels3.Dispose();
                hv_AreInputNumChannels1.Dispose();
                hv_AreInputNumChannels3.Dispose();
                hv_AreInputNumChannels1Or3.Dispose();
                hv_ValidNumChannels.Dispose();
                hv_ValidNumChannelsText.Dispose();
                hv_PreserveDomain.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv_UniqRow1.Dispose();
                hv_UniqColumn1.Dispose();
                hv_UniqRow2.Dispose();
                hv_UniqColumn2.Dispose();
                hv_RectangleIndex.Dispose();
                hv_OriginalWidth.Dispose();
                hv_OriginalHeight.Dispose();
                hv_UniqWidth.Dispose();
                hv_UniqHeight.Dispose();
                hv_ScaleWidth.Dispose();
                hv_ScaleHeight.Dispose();
                hv_ScaleIndex.Dispose();
                hv_ImageIndex.Dispose();
                hv_NumChannels.Dispose();
                hv_ChannelIndex.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();
                hv_Scale.Dispose();
                hv_Shift.Dispose();
                hv_MeanValues.Dispose();
                hv_DeviationValues.Dispose();
                hv_UseDefaultNormalizationValues.Dispose();
                hv_Exception.Dispose();
                hv_Indices.Dispose();
                hv_RescaleRange.Dispose();
                hv_CurrentNumChannels.Dispose();
                hv_DiffNumChannelsIndices.Dispose();
                hv_Index.Dispose();
                hv_DiffNumChannelsIndex.Dispose();
                hv_NumDomains.Dispose();
                hv_DomainIndex.Dispose();

                throw HDevExpDefaultException;
            }
        }




        // Procedures 
        // Chapter: Deep Learning / Model
        // Short Description: Preprocess anomaly images for evaluation and visualization of deep-learning-based anomaly detection or Global Context Anomaly Detection. 
        static public void preprocess_dl_model_anomaly(HObject ho_AnomalyImages, out HObject ho_AnomalyImagesPreprocessed,
            HTuple hv_DLPreprocessParam)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            // Local copy input parameter variables 
            HObject ho_AnomalyImages_COPY_INP_TMP;
            ho_AnomalyImages_COPY_INP_TMP = new HObject(ho_AnomalyImages);



            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_ImageRangeMin = new HTuple(), hv_ImageRangeMax = new HTuple();
            HTuple hv_DomainHandling = new HTuple(), hv_ModelType = new HTuple();
            HTuple hv_ImageNumChannels = new HTuple(), hv_Min = new HTuple();
            HTuple hv_Max = new HTuple(), hv_Range = new HTuple();
            HTuple hv_ImageWidthInput = new HTuple(), hv_ImageHeightInput = new HTuple();
            HTuple hv_EqualWidth = new HTuple(), hv_EqualHeight = new HTuple();
            HTuple hv_Type = new HTuple(), hv_NumMatches = new HTuple();
            HTuple hv_NumImages = new HTuple(), hv_EqualByte = new HTuple();
            HTuple hv_NumChannelsAllImages = new HTuple(), hv_ImageNumChannelsTuple = new HTuple();
            HTuple hv_IndicesWrongChannels = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_AnomalyImagesPreprocessed);
            try
            {
                //
                //This procedure preprocesses the anomaly images given by AnomalyImages
                //according to the parameters in the dictionary DLPreprocessParam.
                //Note that depending on the images,
                //additional preprocessing steps might be beneficial.
                //
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get the preprocessing parameters.
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_ImageRangeMin.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_min", out hv_ImageRangeMin);
                hv_ImageRangeMax.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_max", out hv_ImageRangeMax);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                hv_ModelType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "model_type", out hv_ModelType);
                //
                hv_ImageNumChannels.Dispose();
                hv_ImageNumChannels = 1;
                //
                //Preprocess the images.
                //
                if ((int)(new HTuple(hv_DomainHandling.TupleEqual("full_domain"))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FullDomain(ho_AnomalyImages_COPY_INP_TMP, out ExpTmpOutVar_0
                            );
                        ho_AnomalyImages_COPY_INP_TMP.Dispose();
                        ho_AnomalyImages_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else if ((int)(new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.CropDomain(ho_AnomalyImages_COPY_INP_TMP, out ExpTmpOutVar_0
                            );
                        ho_AnomalyImages_COPY_INP_TMP.Dispose();
                        ho_AnomalyImages_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else if ((int)((new HTuple(hv_DomainHandling.TupleEqual("keep_domain"))).TupleAnd(
                    new HTuple(hv_ModelType.TupleEqual("anomaly_detection")))) != 0)
                {
                    //The option 'keep_domain' is only supported for models of 'type' = 'anomaly_detection'
                }
                else
                {
                    throw new HalconException("Unsupported parameter value for 'domain_handling'");
                }
                //
                hv_Min.Dispose(); hv_Max.Dispose(); hv_Range.Dispose();
                HOperatorSet.MinMaxGray(ho_AnomalyImages_COPY_INP_TMP, ho_AnomalyImages_COPY_INP_TMP,
                    0, out hv_Min, out hv_Max, out hv_Range);
                if ((int)(new HTuple(hv_Min.TupleLess(0.0))) != 0)
                {
                    throw new HalconException("Values of anomaly image must not be smaller than 0.0.");
                }
                //
                //Zoom images only if they have a different size than the specified size.
                hv_ImageWidthInput.Dispose(); hv_ImageHeightInput.Dispose();
                HOperatorSet.GetImageSize(ho_AnomalyImages_COPY_INP_TMP, out hv_ImageWidthInput,
                    out hv_ImageHeightInput);
                hv_EqualWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualWidth = hv_ImageWidth.TupleEqualElem(
                        hv_ImageWidthInput);
                }
                hv_EqualHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualHeight = hv_ImageHeight.TupleEqualElem(
                        hv_ImageHeightInput);
                }
                if ((int)((new HTuple(((hv_EqualWidth.TupleMin())).TupleEqual(0))).TupleOr(
                    new HTuple(((hv_EqualHeight.TupleMin())).TupleEqual(0)))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ZoomImageSize(ho_AnomalyImages_COPY_INP_TMP, out ExpTmpOutVar_0,
                            hv_ImageWidth, hv_ImageHeight, "nearest_neighbor");
                        ho_AnomalyImages_COPY_INP_TMP.Dispose();
                        ho_AnomalyImages_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Check the type of the input images.
                hv_Type.Dispose();
                HOperatorSet.GetImageType(ho_AnomalyImages_COPY_INP_TMP, out hv_Type);
                hv_NumMatches.Dispose();
                HOperatorSet.TupleRegexpTest(hv_Type, "byte|real", out hv_NumMatches);
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_AnomalyImages_COPY_INP_TMP, out hv_NumImages);
                if ((int)(new HTuple(hv_NumMatches.TupleNotEqual(hv_NumImages))) != 0)
                {
                    throw new HalconException("Please provide only images of type 'byte' or 'real'.");
                }
                //
                //If the type is 'byte', convert it to 'real' and scale it.
                //The gray value scaling does not work on 'byte' images.
                //For 'real' images it is assumed that the range is already correct.
                hv_EqualByte.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualByte = hv_Type.TupleEqualElem(
                        "byte");
                }
                if ((int)(new HTuple(((hv_EqualByte.TupleMax())).TupleEqual(1))) != 0)
                {
                    if ((int)(new HTuple(((hv_EqualByte.TupleMin())).TupleEqual(0))) != 0)
                    {
                        throw new HalconException("Passing mixed type images is not supported.");
                    }
                    //Convert the image type from 'byte' to 'real',
                    //because the model expects 'real' images.
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_AnomalyImages_COPY_INP_TMP, out ExpTmpOutVar_0,
                            "real");
                        ho_AnomalyImages_COPY_INP_TMP.Dispose();
                        ho_AnomalyImages_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Check the number of channels.
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_AnomalyImages_COPY_INP_TMP, out hv_NumImages);
                //Check all images for number of channels.
                hv_NumChannelsAllImages.Dispose();
                HOperatorSet.CountChannels(ho_AnomalyImages_COPY_INP_TMP, out hv_NumChannelsAllImages);
                hv_ImageNumChannelsTuple.Dispose();
                HOperatorSet.TupleGenConst(hv_NumImages, hv_ImageNumChannels, out hv_ImageNumChannelsTuple);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_IndicesWrongChannels.Dispose();
                    HOperatorSet.TupleFind(hv_NumChannelsAllImages.TupleNotEqualElem(hv_ImageNumChannelsTuple),
                        1, out hv_IndicesWrongChannels);
                }
                //
                //Check for anomaly image channels.
                //Only single channel images are accepted.
                if ((int)(new HTuple(hv_IndicesWrongChannels.TupleNotEqual(-1))) != 0)
                {
                    throw new HalconException("Number of channels in anomaly image is not supported. Please check for anomaly images with a number of channels different from 1.");
                }
                //
                //Write preprocessed image to output variable.
                ho_AnomalyImagesPreprocessed.Dispose();
                ho_AnomalyImagesPreprocessed = new HObject(ho_AnomalyImages_COPY_INP_TMP);
                //
                ho_AnomalyImages_COPY_INP_TMP.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_ModelType.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();
                hv_ImageWidthInput.Dispose();
                hv_ImageHeightInput.Dispose();
                hv_EqualWidth.Dispose();
                hv_EqualHeight.Dispose();
                hv_Type.Dispose();
                hv_NumMatches.Dispose();
                hv_NumImages.Dispose();
                hv_EqualByte.Dispose();
                hv_NumChannelsAllImages.Dispose();
                hv_ImageNumChannelsTuple.Dispose();
                hv_IndicesWrongChannels.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_AnomalyImages_COPY_INP_TMP.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_ModelType.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();
                hv_ImageWidthInput.Dispose();
                hv_ImageHeightInput.Dispose();
                hv_EqualWidth.Dispose();
                hv_EqualHeight.Dispose();
                hv_Type.Dispose();
                hv_NumMatches.Dispose();
                hv_NumImages.Dispose();
                hv_EqualByte.Dispose();
                hv_NumChannelsAllImages.Dispose();
                hv_ImageNumChannelsTuple.Dispose();
                hv_IndicesWrongChannels.Dispose();

                throw HDevExpDefaultException;
            }
        }


        static public void threshold_dl_anomaly_results(HTuple hv_AnomalySegmentationThreshold,
      HTuple hv_AnomalyClassificationThreshold, HTuple hv_DLResults)
        {



            // Local iconic variables 

            HObject ho_AnomalyImage = null, ho_AnomalyRegion = null;

            // Local control variables 

            HTuple hv_DLResultIndex = new HTuple(), hv_DLResult = new HTuple();
            HTuple hv_DLResultKeys = new HTuple(), hv_ImageKeys = new HTuple();
            HTuple hv_ScoreKeys = new HTuple(), hv_Index = new HTuple();
            HTuple hv_AnomalyImageRegionType = new HTuple(), hv_AnomalyRegionName = new HTuple();
            HTuple hv_AnomalyScoreType = new HTuple(), hv_AnomalyScoreName = new HTuple();
            HTuple hv_AnomalyScore = new HTuple(), hv_AnomalyClassName = new HTuple();
            HTuple hv_AnomalyClassIDName = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_AnomalyImage);
            HOperatorSet.GenEmptyObj(out ho_AnomalyRegion);
            try
            {
                //This procedure applies given thresholds on anomaly detection (AD)
                //or Global Context Anomaly Detection (GC-AD) results.
                //The thresholds are used for:
                //
                //1. Region segmentation: AnomalySegmentationThreshold is used as threshold
                //whether a pixel within the anomaly image belongs to a region of an anomaly.
                //The region is returned in DLResult under one of the following keys, depending on
                //the anomaly image key stored in the DLResult:
                //- 'anomaly_region' (AD, GC_AD)
                //- 'anomaly_region_local' (GC-AD)
                //- 'anomaly_region_global' (GC-AD)
                //2. Image classification: AnomalyClassificationThreshold is used as threshold
                //whether the image is classified as containing an anomaly ('nok' / class_id: 1) or not ('ok' / class_id: 0).
                //The class is returned in DLResult under one of the following keys:
                //- 'anomaly_class' (AD, GC_AD): The classification result as a string ('ok' or 'nok').
                //- 'anomaly_class_local' (GC-AD): The classification result as a string ('ok' or 'nok').
                //- 'anomaly_class_global' (GC-AD): The classification result as a string ('ok' or 'nok').
                //- 'anomaly_class_id' (AD, GC_AD): The classification result as an integer (0 or 1).
                //- 'anomaly_class_id_local' (GC-AD): The classification result as an integer (0 or 1).
                //- 'anomaly_class_id_global' (GC-AD): The classification result as an integer (0 or 1).
                //
                //The applied thresholds are also stored in DLResult.
                //
                //Check for invalid AnomalySegmentationThreshold.
                if ((int)(new HTuple((new HTuple(hv_AnomalySegmentationThreshold.TupleLength()
                    )).TupleNotEqual(1))) != 0)
                {
                    throw new HalconException("AnomalySegmentationThreshold must be specified by exactly one value.");
                }
                //
                //Check for invalid AnomalyClassificationThreshold.
                if ((int)(new HTuple((new HTuple(hv_AnomalyClassificationThreshold.TupleLength()
                    )).TupleNotEqual(1))) != 0)
                {
                    throw new HalconException("AnomalyClassificationThreshold must be specified by exactly one value.");
                }
                //
                //Evaluate each DLResult.
                for (hv_DLResultIndex = 0; (int)hv_DLResultIndex <= (int)((new HTuple(hv_DLResults.TupleLength()
                    )) - 1); hv_DLResultIndex = (int)hv_DLResultIndex + 1)
                {
                    //
                    //Read anomaly image and anomaly score from DLResult.
                    hv_DLResult.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DLResult = hv_DLResults.TupleSelect(
                            hv_DLResultIndex);
                    }
                    hv_DLResultKeys.Dispose();
                    HOperatorSet.GetDictParam(hv_DLResult, "keys", new HTuple(), out hv_DLResultKeys);
                    hv_ImageKeys.Dispose();
                    HOperatorSet.TupleRegexpSelect(hv_DLResultKeys, ".*_image.*", out hv_ImageKeys);
                    hv_ScoreKeys.Dispose();
                    HOperatorSet.TupleRegexpSelect(hv_DLResultKeys, ".*_score.*", out hv_ScoreKeys);
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.TupleSort(hv_ImageKeys, out ExpTmpOutVar_0);
                        hv_ImageKeys.Dispose();
                        hv_ImageKeys = ExpTmpOutVar_0;
                    }
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.TupleSort(hv_ScoreKeys, out ExpTmpOutVar_0);
                        hv_ScoreKeys.Dispose();
                        hv_ScoreKeys = ExpTmpOutVar_0;
                    }
                    if ((int)((new HTuple(hv_ImageKeys.TupleEqual(new HTuple()))).TupleOr(new HTuple(hv_ScoreKeys.TupleEqual(
                        new HTuple())))) != 0)
                    {
                        throw new HalconException(new HTuple("DLResult must contain keys 'anomaly_image' (local, global) and 'anomaly_score' (local, global)."));
                    }
                    for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ImageKeys.TupleLength()
                        )) - 1); hv_Index = (int)hv_Index + 1)
                    {
                        //Apply AnomalyThreshold to the anomaly image.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_AnomalyImage.Dispose();
                            HOperatorSet.GetDictObject(out ho_AnomalyImage, hv_DLResult, hv_ImageKeys.TupleSelect(
                                hv_Index));
                        }
                        ho_AnomalyRegion.Dispose();
                        HOperatorSet.Threshold(ho_AnomalyImage, out ho_AnomalyRegion, hv_AnomalySegmentationThreshold,
                            "max");
                        //
                        //Write AnomalyRegion to DLResult.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_AnomalyImageRegionType.Dispose();
                            HOperatorSet.TupleRegexpMatch(hv_ImageKeys.TupleSelect(hv_Index), "anomaly_image(.*)",
                                out hv_AnomalyImageRegionType);
                        }
                        hv_AnomalyRegionName.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_AnomalyRegionName = "anomaly_region" + hv_AnomalyImageRegionType;
                        }
                        HOperatorSet.SetDictObject(ho_AnomalyRegion, hv_DLResult, hv_AnomalyRegionName);
                        //
                        //Classify sample as 'ok' or 'nok'.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_AnomalyScoreType.Dispose();
                            HOperatorSet.TupleRegexpMatch(hv_ScoreKeys.TupleSelect(hv_Index), "anomaly_score(.*)",
                                out hv_AnomalyScoreType);
                        }
                        hv_AnomalyScoreName.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_AnomalyScoreName = hv_ScoreKeys.TupleSelect(
                                hv_Index);
                        }
                        hv_AnomalyScore.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLResult, hv_AnomalyScoreName, out hv_AnomalyScore);
                        hv_AnomalyClassName.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_AnomalyClassName = "anomaly_class" + hv_AnomalyScoreType;
                        }
                        hv_AnomalyClassIDName.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_AnomalyClassIDName = "anomaly_class_id" + hv_AnomalyScoreType;
                        }
                        if ((int)(new HTuple(hv_AnomalyScore.TupleLess(hv_AnomalyClassificationThreshold))) != 0)
                        {
                            HOperatorSet.SetDictTuple(hv_DLResult, hv_AnomalyClassName, "ok");
                            HOperatorSet.SetDictTuple(hv_DLResult, hv_AnomalyClassIDName, 0);
                        }
                        else
                        {
                            HOperatorSet.SetDictTuple(hv_DLResult, hv_AnomalyClassName, "nok");
                            HOperatorSet.SetDictTuple(hv_DLResult, hv_AnomalyClassIDName, 1);
                        }
                    }
                    //
                    //Write anomaly thresholds to DLResult.
                    HOperatorSet.SetDictTuple(hv_DLResult, "anomaly_classification_threshold",
                        hv_AnomalyClassificationThreshold);
                    HOperatorSet.SetDictTuple(hv_DLResult, "anomaly_segmentation_threshold",
                        hv_AnomalySegmentationThreshold);
                }
                //
                ho_AnomalyImage.Dispose();
                ho_AnomalyRegion.Dispose();

                hv_DLResultIndex.Dispose();
                hv_DLResult.Dispose();
                hv_DLResultKeys.Dispose();
                hv_ImageKeys.Dispose();
                hv_ScoreKeys.Dispose();
                hv_Index.Dispose();
                hv_AnomalyImageRegionType.Dispose();
                hv_AnomalyRegionName.Dispose();
                hv_AnomalyScoreType.Dispose();
                hv_AnomalyScoreName.Dispose();
                hv_AnomalyScore.Dispose();
                hv_AnomalyClassName.Dispose();
                hv_AnomalyClassIDName.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_AnomalyImage.Dispose();
                ho_AnomalyRegion.Dispose();

                hv_DLResultIndex.Dispose();
                hv_DLResult.Dispose();
                hv_DLResultKeys.Dispose();
                hv_ImageKeys.Dispose();
                hv_ScoreKeys.Dispose();
                hv_Index.Dispose();
                hv_AnomalyImageRegionType.Dispose();
                hv_AnomalyRegionName.Dispose();
                hv_AnomalyScoreType.Dispose();
                hv_AnomalyScoreName.Dispose();
                hv_AnomalyScore.Dispose();
                hv_AnomalyClassName.Dispose();
                hv_AnomalyClassIDName.Dispose();

                throw HDevExpDefaultException;
            }
        }



    }
}

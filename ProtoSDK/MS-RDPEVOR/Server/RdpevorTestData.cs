// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.IO;

namespace Microsoft.Protocols.TestTools.StackSdk.RemoteDesktop.Rdpevor
{
    /// <summary>
    /// Data of a sample.
    /// </summary>
    public class Sample
    {
        /// <summary>
        /// the current sample number.  The first sample will have this field set to 1.
        /// </summary>
        public uint sampleNumber;
        /// <summary>
        /// Timestamp of the current packet, in 100-ns intervals since the video presentation was started.
        /// </summary>
        public ulong timeStamp;
        /// <summary>
        /// Encoded sample data.
        /// </summary>
        public byte[] data;
    }
    /// <summary>
    /// All data in this class is used for test.
    /// </summary>
    public class RdpevorTestData
    {
        
        /// <summary>
        /// The geometry mapping id for test.
        /// </summary>
        public UInt64 TestGeometryId;
        
        /// <summary>
        /// The position to left.
        /// </summary>
        public uint TestVideoLeft;

        /// <summary>
        /// The position to top.
        /// </summary>
        public uint TestVideoTop;

        /// <summary>
        /// The scaled video width.
        /// </summary>
        public uint TestVideoWidth;

        /// <summary>
        /// The scaled video height.
        /// </summary>
        public uint TestVideoHeight;

        /// <summary>
        /// List of sample data.
        /// </summary>
        public IList<Sample> sampleList = new List<Sample>();

        /// <summary>
        /// extra data for TSMM_PRESENTATION_REQUEST.
        /// </summary>
        public byte[] extraData;
        
        /// <summary>
        /// Get the subtype id of H264.
        /// </summary>
        public static byte[] GetH264VideoSubtypeId()
        {
            // Only return clone of h264_VideoSubtypeId, so that the h264_VideoSubtypeId is read only. 
            return (byte[])h264_VideoSubtypeId.Clone();         
        }

        /// <summary>
        /// Subtype id of H264, using private protect level so that the value cannot be changed outside
        /// </summary>
        private static byte[] h264_VideoSubtypeId = new byte[]{
            0x48, 0x32, 0x36, 0x34, 0x00, 0x00, 0x10, 0x00,
            0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71
        };
                
        /// <summary>
        /// load video data from a xml file.
        /// </summary>
        public void LoadXMLFile(String path)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.XmlResolver = null;
            XmlReader reader = XmlReader.Create(path, settings);
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;

            doc.Load(reader);

            XmlNodeList elemList = doc.GetElementsByTagName("TestGeometryId");
            if (elemList.Count == 0)
            {
                throw new System.Xml.XmlException("No TestGeometryId node in xml data file:" + path);
            }
            this.TestGeometryId = (ulong)Convert.ToInt64(elemList[0].InnerText, 16);
                    
            elemList = doc.GetElementsByTagName("extraData");
            if (elemList.Count == 0)
            {
                throw new System.Xml.XmlException("No extraData node in xml data file:" + path);
            }
            this.extraData = ParseStringToByteArray(elemList[0].InnerText);

            elemList = doc.GetElementsByTagName("position");
            if (elemList.Count == 0)
            {
                throw new System.Xml.XmlException("No position node in xml data file:" + path);
            }
            foreach (XmlLinkedNode node in elemList[0].ChildNodes)
            {
                if (node.Name.Equals("left"))
                {
                    this.TestVideoLeft = (uint)Int32.Parse(node.InnerText);
                }
                else if (node.Name.Equals("top"))
                {
                    this.TestVideoTop = (uint)Int32.Parse(node.InnerText);
                }
                else if (node.Name.Equals("width"))
                {
                    this.TestVideoWidth = (uint)Int32.Parse(node.InnerText);
                }
                else if (node.Name.Equals("height"))
                {
                    this.TestVideoHeight = (uint)Int32.Parse(node.InnerText);
                }

            }
            elemList = doc.GetElementsByTagName("dataSample");
            if (elemList.Count == 0)
            {
                throw new System.Xml.XmlException("No sample data in xml data file:" + path);
            }
            foreach (XmlLinkedNode dataSample in elemList)
            {
                Sample sample = new Sample();
                foreach (XmlLinkedNode node in dataSample.ChildNodes)
                {
                    if (node.Name.Equals("sampleNumber"))
                    {
                        sample.sampleNumber = (uint)Int32.Parse(node.InnerText);
                    }
                    else if (node.Name.Equals("timeStamp"))
                    {
                        sample.timeStamp = (ulong)Convert.ToInt64(node.InnerText, 16);
                    }
                    else if (node.Name.Equals("data"))
                    {                        
                        sample.data = ParseStringToByteArray(node.InnerText);
                    }
                    
                }
                this.sampleList.Add(sample);
            }

        }

        private byte[] ParseStringToByteArray(String arrayString)
        {
            String[] hexStringArray = arrayString.Split(new char[] { ' ', '\n' });
            List<byte> dataList = new List<byte>();
            for (int i = 0; i < hexStringArray.Length; i++)
            {
                String hexString = hexStringArray[i].Trim(new char[] { ' ', '\n', '\t' });
                if (!hexString.Equals(""))
                    dataList.Add(Convert.ToByte(hexStringArray[i].Trim(), 16));                
            }
            return dataList.ToArray();
        }


        #region Data For Negative test
        //H.264 video data for negative test
        
        /// <summary>
        /// Get the extra properties of H264.
        /// </summary>
        /// <returns></returns>
        public static byte[] GetNegativeTestH264ExtraData()
        {
            // Return a clone of negativeTest_H264_ExtraData, so that negativeTest_H264_ExtraData is readonly
            return (byte[])negativeTest_H264_ExtraData.Clone();
        }

        /// <summary>
        /// The extra properties of H264, using private protect level so that the value cannot be changed outside
        /// </summary>
        private static byte[] negativeTest_H264_ExtraData = new byte[]{
            0x00, 0x00, 0x00, 0x01, 0x67, 0x42, 0xC0, 0x1E, 0x95, 0xA0, 
            0x28, 0x0C, 0xFE, 0x68, 0x40, 0x00, 0x00, 0x03, 0x00, 0x40, 
            0x00, 0x00, 0x0F, 0x03, 0x68, 0x22, 0x11, 0xA8, 0x00, 0x00, 
            0x00, 0x01, 0x68, 0xCE, 0x3C, 0x80
        };

        /// <summary>
        /// The geometry mapping id for test.
        /// </summary>
        public static UInt64 NegativeTestGeometryId = 0x800072B2000A04B4;

        /// <summary>
        /// The position to left.
        /// </summary>
        public static uint NegativeTestVideoLeft = 0;

        /// <summary>
        /// The position to top.
        /// </summary>
        public static uint NegativeTestVideoTop = 0;

        /// <summary>
        /// The scaled video width.
        /// </summary>
        public static uint NegativeTestVideoWidth = 640;

        /// <summary>
        /// The scaled video height.
        /// </summary>
        public static uint NegativeTestVideoHeight = 390;

        /// <summary>
        /// The index of test packet 1.
        /// </summary>
        public static readonly ushort NegativeTest_H264_Packet1Index = 1;

        /// <summary>
        /// The total packets of the sample which contains packet 1.
        /// </summary>
        public static readonly ushort NegativeTest_H264_Packet1TotalPackets = 3;

        /// <summary>
        /// The index of the sample which contains packet 1.
        /// </summary>
        public static readonly uint NegativeTest_H264_Packet1SampleNumber = 1;

        /// <summary>
        /// The timestamp of the sample 1 in packet 1.
        /// </summary>
        public static readonly ulong NegativeTest_H264_Packet1Sample1Timestamp = 0x1856971;

        /// <summary>
        /// Get the H264 packet 1 in sample 1 for test
        /// </summary>
        /// <returns></returns>
        public static byte[] GetNegativeTestH264VideoDataPacket1()
        {
            // Only return clone of negativeTest_H264_VideoDataPacket1, so that negativeTest_H264_VideoDataPacket1 is readonly
            return (byte[])negativeTest_H264_VideoDataPacket1.Clone();
        }

        /// <summary>
        /// The H264 packet 1 in sample 1 for test, using private protect level so that the value cannot be changed outsid
        /// </summary>
        private static byte[] negativeTest_H264_VideoDataPacket1 = new byte[]
        {
            0x00, 0x00, 0x00, 0x01, 0x67, 0x42, 0xC0, 0x1E, 0x95, 0xA0, 0x28, 0x0C, 0xFE, 0x68, 0x40, 0x00, 0x00, 0x03, 0x00, 0x40, 0x00, 0x00, 0x0F, 0x03, 0x68, 
            0x22, 0x11, 0xA8, 0x00, 0x00, 0x00, 0x01, 0x68, 0xCE, 0x3C, 0x80, 0x00, 0x00, 0x00, 0x01, 0x06, 0x05, 0x2F, 0x02, 0xF8, 0x61, 0x50, 0xFC, 0x70, 0x41, 
            0x72, 0xB7, 0x32, 0x48, 0xF3, 0xA7, 0x2A, 0x3D, 0x34, 0x4D, 0x69, 0x63, 0x72, 0x6F, 0x73, 0x6F, 0x66, 0x74, 0x20, 0x48, 0x2E, 0x32, 0x36, 0x34, 0x20, 
            0x45, 0x6E, 0x63, 0x6F, 0x64, 0x65, 0x72, 0x20, 0x56, 0x31, 0x2E, 0x35, 0x2E, 0x33, 0x00, 0x80, 0x00, 0x00, 0x00, 0x01, 0x06, 0x05, 0xF3, 0xCB, 0xB2, 
            0x13, 0x92, 0x98, 0x73, 0x43, 0xDA, 0xA8, 0xA6, 0xC7, 0x42, 0x98, 0x35, 0x6C, 0xF5, 0x73, 0x72, 0x63, 0x3A, 0x33, 0x20, 0x68, 0x3A, 0x33, 0x39, 0x30, 
            0x20, 0x77, 0x3A, 0x36, 0x34, 0x30, 0x20, 0x66, 0x70, 0x73, 0x3A, 0x33, 0x30, 0x2E, 0x30, 0x30, 0x30, 0x20, 0x70, 0x66, 0x3A, 0x36, 0x36, 0x20, 0x6C, 
            0x76, 0x6C, 0x3A, 0x38, 0x20, 0x62, 0x3A, 0x30, 0x20, 0x62, 0x71, 0x70, 0x3A, 0x32, 0x20, 0x67, 0x6F, 0x70, 0x3A, 0x37, 0x35, 0x30, 0x20, 0x69, 0x64, 
            0x72, 0x3A, 0x37, 0x35, 0x30, 0x20, 0x73, 0x6C, 0x63, 0x3A, 0x31, 0x20, 0x63, 0x6D, 0x70, 0x3A, 0x30, 0x20, 0x72, 0x63, 0x3A, 0x31, 0x20, 0x71, 0x70, 
            0x3A, 0x32, 0x34, 0x20, 0x72, 0x61, 0x74, 0x65, 0x3A, 0x34, 0x38, 0x30, 0x30, 0x30, 0x30, 0x30, 0x20, 0x70, 0x65, 0x61, 0x6B, 0x3A, 0x36, 0x34, 0x30, 
            0x30, 0x30, 0x30, 0x30, 0x20, 0x62, 0x75, 0x66, 0x66, 0x3A, 0x38, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x20, 0x72, 0x65, 0x66, 0x3A, 0x31, 0x20, 0x73, 
            0x72, 0x63, 0x68, 0x3A, 0x33, 0x32, 0x20, 0x61, 0x73, 0x72, 0x63, 0x68, 0x3A, 0x31, 0x20, 0x73, 0x75, 0x62, 0x70, 0x3A, 0x31, 0x20, 0x70, 0x61, 0x72, 
            0x3A, 0x36, 0x20, 0x33, 0x20, 0x33, 0x20, 0x72, 0x6E, 0x64, 0x3A, 0x30, 0x20, 0x63, 0x61, 0x62, 0x61, 0x63, 0x3A, 0x30, 0x20, 0x6C, 0x70, 0x3A, 0x32, 
            0x20, 0x63, 0x74, 0x6E, 0x74, 0x3A, 0x30, 0x20, 0x61, 0x75, 0x64, 0x3A, 0x31, 0x20, 0x6C, 0x61, 0x74, 0x3A, 0x31, 0x20, 0x77, 0x72, 0x6B, 0x3A, 0x31, 
            0x20, 0x76, 0x75, 0x69, 0x3A, 0x31, 0x20, 0x6C, 0x79, 0x72, 0x3A, 0x31, 0x20, 0x3C, 0x3C, 0x00, 0x80, 0x00, 0x00, 0x00, 0x01, 0x09, 0x10, 0x00, 0x00, 
            0x00, 0x01, 0x65, 0x88, 0x80, 0x4B, 0xFF, 0xFF, 0xF0, 0xF4, 0x50, 0x00, 0x10, 0x23, 0xF7, 0x8A, 0xDF, 0x7D, 0xF7, 0xDF, 0x7D, 0xF7, 0xDF, 0x7D, 0xF7, 
            0xDF, 0x7D, 0xF7, 0xDF, 0x7D, 0xF7, 0xDF, 0x7D, 0xF7, 0xDF, 0x7D, 0xF7, 0xDF, 0x7D, 0xF7, 0xDF, 0x7D, 0xF7, 0xDF, 0x7F, 0xF2, 0x2C, 0x8B, 0xF2, 0x2E, 
            0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 
            0xEB, 0xAE, 0xBA, 0xE4, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 
            0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 
            0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 
            0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 
            0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 
            0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 
            0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 
            0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 
            0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 
            0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 
            0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAF, 0xFF, 0xE8, 0xA0, 0xCF, 0x19, 0xE0, 0x54, 0x71, 0x61, 0x03, 0x5C, 0x17, 0x63, 0x95, 0x39, 0x9A, 0x49, 
            0xC6, 0xA3, 0xA8, 0x78, 0xAB, 0xAA, 0xDB, 0x17, 0xD1, 0xDF, 0x27, 0x93, 0x30, 0x71, 0xB7, 0xFF, 0x16, 0xCC, 0x9B, 0xD7, 0x5D, 0x75, 0xD7, 0x5D, 0x75, 
            0xD7, 0x5D, 0x75, 0xD7, 0x5D, 0x75, 0xD7, 0x5D, 0x75, 0xD7, 0x5D, 0x75, 0xD7, 0x5D, 0x75, 0xD7, 0x5D, 0x75, 0xD7, 0x5D, 0x75, 0xD7, 0x5E, 0x42, 0x92, 
            0xB4, 0x46, 0x8B, 0x6D, 0xC0, 0x03, 0xF2, 0x2E, 0x05, 0xCA, 0xFC, 0x50, 0x91, 0xE5, 0x81, 0x89, 0x23, 0x8C, 0x2E, 0x52, 0x28, 0x01, 0x6B, 0x65, 0xC8, 
            0xCC, 0x6A, 0x4C, 0x9F, 0xFD, 0x9B, 0x52, 0x98, 0x3E, 0x86, 0x77, 0xC9, 0x06, 0x86, 0x8C, 0x72, 0x91, 0x3D, 0xF7, 0xBA, 0xC9, 0x24, 0xA3, 0xDE, 0x1D, 
            0xF0, 0x01, 0xEB, 0x37, 0x4B, 0x0C, 0xFE, 0x88, 0xF3, 0x08, 0xF4, 0x34, 0x61, 0x25, 0x47, 0x36, 0xDE, 0x1B, 0xF5, 0xD0, 0x05, 0x9F, 0x5D, 0x92, 0x56, 
            0xB6, 0xB5, 0x2A, 0xB7, 0x62, 0xA8, 0x4D, 0x65, 0x0C, 0xC7, 0xF7, 0x1A, 0xF4, 0xBF, 0x5D, 0xAF, 0x5D, 0x75, 0xD7, 0x5D, 0x75, 0xD7, 0x5D, 0x75, 0xD7, 
            0x5D, 0x75, 0xD7, 0x5D, 0x75, 0xD7, 0x5D, 0x75, 0xD7, 0x5D, 0x75, 0xD7, 0x5D, 0x75, 0xD7, 0x5D, 0x75, 0xD7, 0x5F, 0xF3, 0xFF, 0xE4, 0xBB, 0xE7, 0xFF, 
            0xE1, 0xE8, 0x4D, 0xF0, 0xEA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 
            0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 
            0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 
            0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 
            0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 
            0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 
            0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 
            0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE, 0xBA, 0xEB, 0xAE
        };

        #endregion
    }
}

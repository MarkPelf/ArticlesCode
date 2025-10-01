using DataTables.AspNet.AspNetCore;
using Newtonsoft.Json;
using System.Collections;

//DataTablesResponseHelper.cs
namespace Example06
{
    /*
     * This code has been taken from https://github.com/ALMMa/datatables.aspnet/issues/72
     * It resolves the bug in library DataTables.AspNet.AspNetCore
     * when returning parameters from server to client
     * proper usage is as follows
     * 
     *  // Create some response additional parameters.
     *   var returnParameters = new Dictionary<string, object>()
     *               {
     *                   { "rParam1", "My parameter" },
     *                   { "rParam2", 12345 }
     *               };
     *
     *  var response = DataTablesResponse.Create(
     *       request, totalRecordsCount, filteredRecordsCount, dataPage, returnParameters);
     *  var response2 = new DataTablesResponseHelper(response);
     *
     *  return new DataTablesJsonResult(response2, true);
     */


    public class DataTablesResponseHelper : DataTables.AspNet.AspNetCore.DataTablesResponse
    {
        protected DataTablesResponseHelper(int draw, string errorMessage) : 
            base(draw, errorMessage)
        {
        }

        protected DataTablesResponseHelper(int draw, string errorMessage, 
            IDictionary<string, object> additionalParameters) : 
            base(draw, errorMessage, additionalParameters)
        {
        }

        protected DataTablesResponseHelper(int draw, int totalRecords, int totalRecordsFiltered, object data) : 
            base(draw, totalRecords, totalRecordsFiltered, data)
        {
        }

        protected DataTablesResponseHelper(int draw, int totalRecords, int totalRecordsFiltered, 
            object data, IDictionary<string, object> additionalParameters) : 
            base(draw, totalRecords, totalRecordsFiltered, data, additionalParameters)
        {
        }

        public DataTablesResponseHelper(DataTablesResponse response) : 
            this(response.Draw, response.TotalRecords, response.TotalRecordsFiltered, 
                response.Data, response.AdditionalParameters)
        {
        }

        private bool IsSuccessResponse()
        {
            return this.Data != null && string.IsNullOrWhiteSpace(this.Error);
        }

        public override string ToString()
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                using (JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter)stringWriter))
                {
                    if (this.IsSuccessResponse())
                    {
                        jsonTextWriter.WriteStartObject();
                        jsonTextWriter.WritePropertyName(Configuration.Options.ResponseNameConvention.Draw, true);
                        jsonTextWriter.WriteValue(this.Draw);
                        jsonTextWriter.WritePropertyName(Configuration.Options.ResponseNameConvention.TotalRecords, true);
                        jsonTextWriter.WriteValue(this.TotalRecords);
                        jsonTextWriter.WritePropertyName(Configuration.Options.ResponseNameConvention.TotalRecordsFiltered, true);
                        jsonTextWriter.WriteValue(this.TotalRecordsFiltered);
                        jsonTextWriter.WritePropertyName(Configuration.Options.ResponseNameConvention.Data, true);
                        jsonTextWriter.WriteRawValue(this.SerializeData(this.Data));
                        if (this.AdditionalParameters != null)
                        {
                            foreach (KeyValuePair<string, object> additionalParameter in (IEnumerable<KeyValuePair<string, object>>)this.AdditionalParameters)
                            {
                                jsonTextWriter.WritePropertyName(additionalParameter.Key, true);
                                if (additionalParameter.Value is IEnumerable)
                                {
                                    jsonTextWriter.WriteRawValue(SerializeData(additionalParameter.Value));
                                }
                                else
                                {
                                    jsonTextWriter.WriteValue(additionalParameter.Value);
                                }
                            }
                        }
                        jsonTextWriter.WriteEndObject();
                    }
                    else
                    {
                        jsonTextWriter.WriteStartObject();
                        jsonTextWriter.WritePropertyName(Configuration.Options.ResponseNameConvention.Draw, true);
                        jsonTextWriter.WriteValue(this.Draw);
                        jsonTextWriter.WritePropertyName(Configuration.Options.ResponseNameConvention.Error, true);
                        jsonTextWriter.WriteValue(this.Error);
                        if (this.AdditionalParameters != null)
                        {
                            foreach (KeyValuePair<string, object> additionalParameter in 
                                (IEnumerable<KeyValuePair<string, object>>) this.AdditionalParameters)
                            {
                                jsonTextWriter.WritePropertyName(additionalParameter.Key, true);
                                if (additionalParameter.Value is IEnumerable)
                                {
                                    jsonTextWriter.WriteRawValue(SerializeData(additionalParameter.Value));
                                }
                                else
                                {
                                    jsonTextWriter.WriteValue(additionalParameter.Value);
                                }
                            }
                        }
                        jsonTextWriter.WriteEndObject();
                    }
                    jsonTextWriter.Flush();
                    return stringWriter.ToString();
                }
            }
        }
    }
}

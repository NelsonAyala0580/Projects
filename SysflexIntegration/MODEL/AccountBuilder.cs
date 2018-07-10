using Log4NetMR;
using SysflexIntegrationUTIL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODEL
{
    public class AccountBuilder
    {

        ///  ACTNUMBR_1 ACTNUMBR_2  ACTNUMBR_3 ACTNUMBR_4  ACTNUMBR_5 ACTNUMBR_6  ACTNUMBR_7 ACTNUMBR_8  ACTNUMBR_9
        ///    1105   	 00     	  000    	0000   	      00       	00       	171     	00001  	   01     
        ///    

        classClsRegistrarLog log = new classClsRegistrarLog();
        public string BuildARAccount(int typeAccount, string line_of_business, string policy_type, string vehicle_type, string country, string channel, string supervisor, int? supervisor_code, string GpProductID)
        {
            //var contexto = new ATLANEntities();
            var Account = string.Empty;
            var Segment1 = string.Empty;
            var Segment2 = string.Empty;
            var Segment3 = string.Empty;
            var Segment4 = string.Empty;
            var Segment5 = "00";
            var Segment6 = string.Empty;
            var Segment7 = string.Empty;
            var Segment8 = string.Empty;
            var Segment9 = "00";

            try
            {
                Segment3 = GetLineOfBusinessMapping(line_of_business.Trim());

                //if (Segment3 == "201" || Segment3 == "202")
                //{
                //    Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                //}
                //else
                //{
                //    Segment4 = GpProductID.Trim();
                //}

                if (string.IsNullOrEmpty(GpProductID) || string.IsNullOrWhiteSpace(GpProductID))
                {
                    Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                }
                else
                {
                    Segment4 = GpProductID.Trim();
                }


                Segment1 = GetSegment1(Segment3, Segment4, typeAccount);
                Segment2 = GetSegment2(Segment3, Segment4, typeAccount);
                //Segment6 = GetZoneMapping(zone);
                Segment7 = GetCountryMapping(country).Trim() + GetChannelMapping(channel).Trim();

                if (supervisor_code.HasValue)
                {
                    Segment6 = GetZoneMappingByCode(supervisor_code.GetValueOrDefault());
                    Segment8 = GetSupervisorMappingByCode(supervisor_code.GetValueOrDefault());
                }
                else
                {
                    Segment6 = GetZoneMapping(supervisor.Trim());
                    Segment8 = GetSupervisorMapping(supervisor.Trim());
                }
                Account = Segment1.Trim() + "-" +
                          Segment2.Trim() + "-" +
                          Segment3.Trim() + "-" +
                          Segment4.Trim() + "-" +
                          Segment5.Trim() + "-" +
                          Segment6.Trim() + "-" +
                          Segment7.Trim() + "-" +
                          Segment8.Trim() + "-" +
                          Segment9.Trim();


                if (string.IsNullOrEmpty(Account))
                {
                    log.WriteLog(1, "La cuenta no se ha completado correctamente");
                }
                else
                {
                    CreateAccount(Account);
                }
                return Account;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            finally
            {
                Account = string.Empty;
            }


        }

        /// <summary>
        /// Contruye la  cuenta contable para Ventas (items de invoice)
        /// </summary>
        /// <param name="typeAccount"></param>
        /// <param name="line_of_business"></param>
        /// <param name="policy_type"></param>
        /// <param name="vehicle_type"></param>
        /// <param name="zone"></param>
        /// <param name="country"></param>
        /// <param name="channel"></param>
        /// <param name="supervisor"></param>
        /// <param name="coverage"></param>
        /// <returns></returns>
        public string BuildSalesAccount(int typeAccount, string line_of_business, string policy_type, string vehicle_type, string country, string channel, string supervisor, int? supervisor_code, string coverage, string GpProductID)
        {
            //var contexto = new ATLANEntities();
            var Account = string.Empty;
            var Segment1 = string.Empty;
            var Segment2 = string.Empty;
            var Segment3 = string.Empty;
            var Segment4 = string.Empty;
            var Segment5 = "00";
            var Segment6 = string.Empty;
            var Segment7 = string.Empty;
            var Segment8 = string.Empty;
            var Segment9 = string.Empty;

            try
            {
                Segment3 = GetLineOfBusinessMapping(line_of_business.Trim());
                //Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                //if (Segment3 == "201" || Segment3 == "202")
                //{
                //    Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                //}
                //else
                //{
                //    Segment4 = GpProductID.Trim();
                //}


                if (string.IsNullOrEmpty(GpProductID) || string.IsNullOrWhiteSpace(GpProductID))
                {
                    Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                }
                else
                {
                    Segment4 = GpProductID.Trim();
                }

                Segment1 = GetSegment1(Segment3, Segment4, typeAccount);
                Segment2 = GetSegment2(Segment3, Segment4, typeAccount);
                //Segment6 = GetZoneMapping(supervisor.Trim());
                Segment7 = GetCountryMapping(country).Trim() + GetChannelMapping(channel).Trim();
                //Segment8 = GetSupervisorMapping(supervisor);

                if (supervisor_code.HasValue)
                {
                    Segment6 = GetZoneMappingByCode(supervisor_code.GetValueOrDefault());
                    Segment8 = GetSupervisorMappingByCode(supervisor_code.GetValueOrDefault());
                }
                else
                {
                    Segment6 = GetZoneMapping(supervisor.Trim());
                    Segment8 = GetSupervisorMapping(supervisor.Trim());
                }

                Segment9 = GetCoverageMapping(coverage);

                Account = Segment1.Trim() + "-" +
                          Segment2.Trim() + "-" +
                          Segment3.Trim() + "-" +
                          Segment4.Trim() + "-" +
                          Segment5.Trim() + "-" +
                          Segment6.Trim() + "-" +
                          Segment7.Trim() + "-" +
                          Segment8.Trim() + "-" +
                          Segment9.Trim();



                if (string.IsNullOrEmpty(Account))
                {
                    log.WriteLog(1, "La cuenta no se ha completado correctamente");
                }
                else
                {
                    CreateAccount(Account);
                }
                return Account;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            finally
            {
                Account = string.Empty;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeAccount"></param>
        /// <param name="line_of_business"></param>
        /// <param name="policy_type"></param>
        /// <param name="vehicle_type"></param>
        /// <param name="GpProductID"></param>
        /// <returns></returns>
        public string BuildUnitAccount(int typeAccount, string line_of_business, string policy_type, string vehicle_type, string GpProductID)
        {
            //var contexto = new ATLANEntities();
            var Account = string.Empty;
            var Segment1 = string.Empty;
            var Segment2 = string.Empty;
            var Segment3 = string.Empty;
            var Segment4 = string.Empty;


            try
            {
                Segment3 = GetLineOfBusinessMapping(line_of_business.Trim());
                //Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                //if (Segment3 == "201")
                //{
                //    Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                //}
                //else
                //{
                //    Segment4 = GpProductID.Trim();
                //}

                if (string.IsNullOrEmpty(GpProductID) || string.IsNullOrWhiteSpace(GpProductID))
                {
                    Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                }
                else
                {
                    Segment4 = GpProductID.Trim();
                }

                Segment1 = GetSegment1(Segment3, Segment4, typeAccount);
                Segment2 = GetSegment2(Segment3, Segment4, typeAccount);


                Account = GetUnitAccount(typeAccount, Segment1, Segment2, Segment3, Segment4);



                if (string.IsNullOrEmpty(Account))
                {
                    log.WriteLog(1, "La cuenta no se ha completado correctamente");
                }
                else
                {
                    CreateAccount(Account);
                }
                return Account;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            finally
            {
                Account = string.Empty;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeAccount"></param>
        /// <returns></returns>
        public string BuildUnitTaxAccount(int typeAccount)
        {
            var contexto = new ATLANEntities();
            try
            {
                var UnitTaxAccount = (from accSetting in contexto.ST_GL_MAPPING_ACCOUNT_SETTINGS
                                  where accSetting.TYPEID == typeAccount
                                  select accSetting.UNIT_ACCOUNT1.Trim()).FirstOrDefault().Trim();

                return UnitTaxAccount;
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: No se encuetra la cuenta Unitaria de Impuestos ", 2, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: No se encuetra la cuenta Unitaria de Impuestos ", 2, ex);
                throw;
            }
            finally
            {
                contexto.Dispose();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeAccount"></param>
        /// <param name="line_of_business"></param>
        /// <param name="policy_type"></param>
        /// <param name="vehicle_type"></param>
        /// <param name="zone"></param>
        /// <param name="country"></param>
        /// <param name="channel"></param>
        /// <param name="supervisor"></param>
        /// <param name="supervisor_code"></param>
        /// <param name="coverage"></param>
        /// <param name="GpProductID"></param>
        /// <param name="IsLocal"></param>
        /// <param name="IsFacult"></param>
        /// <returns></returns>
        public string BuildReinsuranceAccount(int typeAccount, string line_of_business, string policy_type, string vehicle_type,
                                              string country, string channel, string supervisor, int? supervisor_code,
                                              string coverage, string GpProductID, bool IsLocal, bool IsFacult)
        {
            //var contexto = new ATLANEntities();
            var Account = string.Empty;
            var Segment1 = string.Empty;
            var Segment2 = string.Empty;
            var Segment3 = string.Empty;
            var Segment4 = string.Empty;
            var Segment5 = "00";
            var Segment6 = string.Empty;
            var Segment7 = string.Empty;
            var Segment8 = string.Empty;
            var Segment9 = string.Empty;

            try
            {
                Segment3 = GetLineOfBusinessMapping(line_of_business.Trim());
                //Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                //if (Segment3 == "201" || Segment3 == "202")
                //{
                //    Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                //}
                //else
                //{
                //    Segment4 = GpProductID.Trim();
                //}

                if (string.IsNullOrEmpty(GpProductID) || string.IsNullOrWhiteSpace(GpProductID))
                {
                    Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                }
                else
                {
                    Segment4 = GpProductID.Trim();
                }

                //Segment1 = GetSegment1(Segment3, Segment4, typeAccount);
                Segment1 = GetSegment1Reinsurance(Segment3, Segment4, IsLocal, IsFacult);
                Segment2 = GetSegment2(Segment3, Segment4, typeAccount);
                //Segment6 = GetZoneMapping(supervisor.Trim());
                Segment7 = GetCountryMapping(country).Trim() + GetChannelMapping(channel).Trim();
                //Segment8 = GetSupervisorMapping(supervisor);

                if (supervisor_code.HasValue)
                {
                    Segment6 = GetZoneMappingByCode(supervisor_code.GetValueOrDefault());
                    Segment8 = GetSupervisorMappingByCode(supervisor_code.GetValueOrDefault());
                }
                else
                {
                    Segment6 = GetZoneMapping(supervisor.Trim());
                    Segment8 = GetSupervisorMapping(supervisor.Trim());
                }

                Segment9 = GetCoverageMapping(coverage);

                Account = Segment1.Trim() + "-" +
                          Segment2.Trim() + "-" +
                          Segment3.Trim() + "-" +
                          Segment4.Trim() + "-" +
                          Segment5.Trim() + "-" +
                          Segment6.Trim() + "-" +
                          Segment7.Trim() + "-" +
                          Segment8.Trim() + "-" +
                          Segment9.Trim();



                if (string.IsNullOrEmpty(Account))
                {
                    log.WriteLog(1, "La cuenta no se ha completado correctamente");
                }
                else
                {
                    CreateAccount(Account);
                }
                return Account;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            finally
            {
                Account = string.Empty;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeAccount"></param>
        /// <param name="line_of_business"></param>
        /// <param name="policy_type"></param>
        /// <param name="vehicle_type"></param>
        /// <param name="zone"></param>
        /// <param name="country"></param>
        /// <param name="channel"></param>
        /// <param name="supervisor"></param>
        /// <param name="supervisor_code"></param>
        /// <param name="coverage"></param>
        /// <param name="GpProductID"></param>
        /// <param name="IsFacult"></param>
        /// <returns></returns>
        public string BuildReinsuranceCommAccount(int typeAccount, string line_of_business, string policy_type, string vehicle_type, string country, string channel, 
                                                  string supervisor, int? supervisor_code,  string coverage, string GpProductID, bool IsFacult)
        {
           // var contexto = new ATLANEntities();

            var Account = string.Empty;
            var Segment1 = string.Empty;
            var Segment2 = string.Empty;
            var Segment3 = string.Empty;
            var Segment4 = string.Empty;
            var Segment5 = "00";
            var Segment6 = string.Empty;
            var Segment7 = string.Empty;
            var Segment8 = string.Empty;
            var Segment9 = string.Empty;

            if (!IsFacult)
            {
                //Segment1 = ConfigKey.ReadSetting("COMREA").Trim();
                typeAccount = 16;
                //Segment1 = GetSegment1(Segment3, Segment4, 16);
            }
            else
            {
                // Segment1 = ConfigKey.ReadSetting("COMFAC").Trim();
                //Segment1 = GetSegment1(Segment3, Segment4, 17);

                typeAccount = 17;
            }

            try
            {

                Segment3 = GetLineOfBusinessMapping(line_of_business.Trim());
                //Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                //if (Segment3 == "201" || Segment3 == "202")
                //{
                //    Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                //}
                //else
                //{
                //    Segment4 = GpProductID.Trim();
                //}


                if (string.IsNullOrEmpty(GpProductID) || string.IsNullOrWhiteSpace(GpProductID))
                {
                    Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                }
                else
                {
                    Segment4 = GpProductID.Trim();
                }

                //Segment1 = ConfigKey.ReadSetting("COMREA").Trim();

                Segment1 = GetSegment1(Segment3, Segment4, typeAccount);


                //Segment1 = GetSegment1(Segment3, Segment4, typeAccount);

                Segment2 = GetSegment2(Segment3, Segment4, typeAccount);
                //Segment6 = GetZoneMapping(supervisor.Trim());
                Segment7 = GetCountryMapping(country).Trim() + GetChannelMapping(channel).Trim();
                //Segment8 = GetSupervisorMapping(supervisor);

                if (supervisor_code.HasValue)
                {
                    Segment6 = GetZoneMappingByCode(supervisor_code.GetValueOrDefault());
                    Segment8 = GetSupervisorMappingByCode(supervisor_code.GetValueOrDefault());
                }
                else
                {
                    Segment6 = GetZoneMapping(supervisor.Trim());
                    Segment8 = GetSupervisorMapping(supervisor.Trim());
                }

                Segment9 = GetCoverageMapping(coverage);

                Account = Segment1.Trim() + "-" +
                          Segment2.Trim() + "-" +
                          Segment3.Trim() + "-" +
                          Segment4.Trim() + "-" +
                          Segment5.Trim() + "-" +
                          Segment6.Trim() + "-" +
                          Segment7.Trim() + "-" +
                          Segment8.Trim() + "-" +
                          Segment9.Trim();



                if (string.IsNullOrEmpty(Account))
                {
                    log.WriteLog(1, "La cuenta no se ha completado correctamente");
                }
                else
                {
                    CreateAccount(Account);
                }
                return Account;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            finally
            {
                Account = string.Empty;
            }
        }


        /// <summary>
        /// Contruye la cuenta contable para Comisiones
        /// </summary>
        /// <param name="typeAccount"></param>
        /// <param name="line_of_business"></param>
        /// <param name="policy_type"></param>
        /// <param name="vehicle_type"></param>
        /// <param name="zone"></param>
        /// <param name="country"></param>
        /// <param name="channel"></param>
        /// <param name="supervisor"></param>
        /// <returns></returns>
        public string BuildAccountCommission(int typeAccount, string line_of_business, string policy_type, string vehicle_type, string country, string channel, string supervisor, int? supervisor_code, string coverage, string GpProductID)
        {
            ///var contexto = new ATLANEntities();
            var Account = string.Empty;
            var Segment1 = string.Empty;
            var Segment2 = string.Empty;
            var Segment3 = string.Empty;
            var Segment4 = string.Empty;
            var Segment5 = "00";
            var Segment6 = string.Empty;
            var Segment7 = string.Empty;
            var Segment8 = string.Empty;
            var Segment9 = string.Empty;

            try
            {
                Segment3 = GetLineOfBusinessMapping(line_of_business.Trim());
                //Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                //if (Segment3 == "201" || Segment3 == "202")
                //{
                //    Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                //}
                //else
                //{
                //    Segment4 = GpProductID.Trim();
                //}

                if (string.IsNullOrEmpty(GpProductID) || string.IsNullOrWhiteSpace(GpProductID))
                {
                    Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                }
                else
                {
                    Segment4 = GpProductID.Trim();
                }

                Segment1 = GetSegment1(Segment3, Segment4, typeAccount);
                Segment2 = GetSegment2(Segment3, Segment4, typeAccount);
                //Segment6 = GetZoneMapping(zone);
                Segment7 = GetCountryMapping(country).Trim() + GetChannelMapping(channel).Trim();
                //Segment8 = GetSupervisorMapping(supervisor);

                if (supervisor_code.HasValue)
                {
                    Segment6 = GetZoneMappingByCode(supervisor_code.GetValueOrDefault());
                    Segment8 = GetSupervisorMappingByCode(supervisor_code.GetValueOrDefault());
                }
                else
                {
                    Segment6 = GetZoneMapping(supervisor.Trim());
                    Segment8 = GetSupervisorMapping(supervisor.Trim());
                }


                Segment9 = GetCoverageMapping(coverage);// "00";

                Account = Segment1.Trim() + "-" +
                          Segment2.Trim() + "-" +
                          Segment3.Trim() + "-" +
                          Segment4.Trim() + "-" +
                          Segment5.Trim() + "-" +
                          Segment6.Trim() + "-" +
                          Segment7.Trim() + "-" +
                          Segment8.Trim() + "-" +
                          Segment9.Trim();



                if (string.IsNullOrEmpty(Account))
                {
                    log.WriteLog(1, "La cuenta no se ha completado correctamente");
                }
                else
                {
                    CreateAccount(Account);
                }

                return Account;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            finally
            {
                Account = string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeAccount"></param>
        /// <param name="line_of_business"></param>
        /// <param name="policy_type"></param>
        /// <param name="vehicle_type"></param>
        /// <param name="country"></param>
        /// <param name="channel"></param>
        /// <param name="supervisor"></param>
        /// <param name="supervisor_code"></param>
        /// <param name="GpProductID"></param>
        /// <returns></returns>
        public string BuildAccountCommission(int typeAccount, string line_of_business, string policy_type, string vehicle_type,  string country, string channel, string supervisor, int? supervisor_code, string GpProductID)
        {
            //var contexto = new ATLANEntities();
            var Account = string.Empty;
            var Segment1 = string.Empty;
            var Segment2 = string.Empty;
            var Segment3 = string.Empty;
            var Segment4 = string.Empty;
            var Segment5 = "00";
            var Segment6 = string.Empty;
            var Segment7 = string.Empty;
            var Segment8 = string.Empty;
            var Segment9 = string.Empty;

            try
            {
                Segment3 = GetLineOfBusinessMapping(line_of_business.Trim());
                //Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                //if (Segment3 == "201" || Segment3 == "202")
                //{
                //    Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                //}
                //else
                //{
                //    Segment4 = GpProductID.Trim();
                //}

                if (string.IsNullOrEmpty(GpProductID) || string.IsNullOrWhiteSpace(GpProductID))
                {
                    Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                }
                else
                {
                    Segment4 = GpProductID.Trim();
                }


                Segment1 = GetSegment1(Segment3, Segment4, typeAccount);
                Segment2 = GetSegment2(Segment3, Segment4, typeAccount);
                //Segment6 = GetZoneMapping(zone);
                Segment7 = GetCountryMapping(country).Trim() + GetChannelMapping(channel).Trim();
                //Segment8 = GetSupervisorMapping(supervisor);

                if (supervisor_code.HasValue)
                {
                    Segment6 = GetZoneMappingByCode(supervisor_code.GetValueOrDefault());
                    Segment8 = GetSupervisorMappingByCode(supervisor_code.GetValueOrDefault());
                }
                else
                {
                    Segment6 = GetZoneMapping(supervisor.Trim());
                    Segment8 = GetSupervisorMapping(supervisor.Trim());
                }


                Segment9 = "00";

                Account = Segment1.Trim() + "-" +
                          Segment2.Trim() + "-" +
                          Segment3.Trim() + "-" +
                          Segment4.Trim() + "-" +
                          Segment5.Trim() + "-" +
                          Segment6.Trim() + "-" +
                          Segment7.Trim() + "-" +
                          Segment8.Trim() + "-" +
                          Segment9.Trim();



                if (string.IsNullOrEmpty(Account))
                {
                    log.WriteLog(1, "La cuenta no se ha completado correctamente");
                }
                else
                {
                    CreateAccount(Account);
                }

                return Account;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            finally
            {
                Account = string.Empty;
            }
        }

        /// <summary>
        /// Construye la cuenta contable para RRC (Reserva de Riesgos en curso)
        /// </summary>
        /// <param name="typeAccount"></param>
        /// <param name="line_of_business"></param>
        /// <param name="policy_type"></param>
        /// <param name="vehicle_type"></param>
        /// <param name="zone"></param>
        /// <param name="country"></param>
        /// <param name="channel"></param>
        /// <param name="supervisor"></param>
        /// <returns></returns>
        public string BuildAccountRRC(int typeAccount, string line_of_business, string policy_type, string vehicle_type, string country, string channel, string supervisor, int? supervisor_code, string GpProductID, string coverage)
        {
            //var contexto = new ATLANEntities();
            var Account = string.Empty;
            var Segment1 = string.Empty;
            var Segment2 = string.Empty;
            var Segment3 = string.Empty;
            var Segment4 = string.Empty;
            var Segment5 = "00";
            var Segment6 = string.Empty;
            var Segment7 = string.Empty;
            var Segment8 = string.Empty;
            var Segment9 = string.Empty;

            try
            {
                Segment3 = GetLineOfBusinessMapping(line_of_business.Trim());
                //Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                //if (Segment3 == "201" || Segment3 == "202")
                //{
                //    Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                //}
                //else
                //{
                //    Segment4 = GpProductID.Trim();
                //}

                if (string.IsNullOrEmpty(GpProductID) || string.IsNullOrWhiteSpace(GpProductID))
                {
                    Segment4 = GetProductMapping(policy_type.Trim(), vehicle_type.Trim());
                }
                else
                {
                    Segment4 = GpProductID.Trim();
                }

                Segment1 = GetSegment1(Segment3, Segment4, typeAccount);
                Segment2 = GetSegment2(Segment3, Segment4, typeAccount);
                //Segment6 = GetZoneMapping(zone);
                Segment7 = GetCountryMapping(country).Trim() + GetChannelMapping(channel).Trim();
                //Segment8 = GetSupervisorMapping(supervisor);

                if (supervisor_code.HasValue)
                {
                    Segment6 = GetZoneMappingByCode(supervisor_code.GetValueOrDefault());
                    Segment8 = GetSupervisorMappingByCode(supervisor_code.GetValueOrDefault());
                }
                else
                {
                    Segment6 = GetZoneMapping(supervisor.Trim());
                    Segment8 = GetSupervisorMapping(supervisor.Trim());
                }

                //Segment9 = "00";
                Segment9 = GetCoverageMapping(coverage);

                Account = Segment1.Trim() + "-" +
                          Segment2.Trim() + "-" +
                          Segment3.Trim() + "-" +
                          Segment4.Trim() + "-" +
                          Segment5.Trim() + "-" +
                          Segment6.Trim() + "-" +
                          Segment7.Trim() + "-" +
                          Segment8.Trim() + "-" +
                          Segment9.Trim();



                if (string.IsNullOrEmpty(Account))
                {
                    log.WriteLog(1, "La cuenta no se ha completado correctamente");
                }
                else
                {
                    CreateAccount(Account);
                }
                return Account;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            finally
            {
                Account = string.Empty;
            }
        }


        /// <summary>
        /// Obtiene el codigo para linea de negocio
        /// </summary>
        /// <param name="line_of_business"></param>
        /// <returns></returns>
        private string GetLineOfBusinessMapping(string line_of_business)
        {
            var contexto = new ATLANEntities();
            var lbmapping = string.Empty;
            try
            {
                lbmapping = (from map in contexto.ST_GL_MAPPING_LINE_OF_BUSINESS
                             where map.LINE_OF_BUSINESS.Trim() == line_of_business.Trim()
                             select map.GL_MAPPING.Trim()).FirstOrDefault();


                if (string.IsNullOrEmpty(lbmapping))
                {
                    log.WriteLog(1, "Póliza no tiene Linea de Negocio Asignado");
                }

                return lbmapping;
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            finally
            {
                contexto.Dispose();
            }

        }

        /// <summary>
        /// Obtiene el codigo del producto
        /// </summary>
        /// <param name="policy_type"></param>
        /// <param name="vehicle_type"></param>
        /// <returns></returns>
        public string GetProductMapping(string policy_type, string vehicle_type)
        {

            ///vehicletype + policytype
            var contexto = new ATLANEntities();
            var ptmapping = string.Empty;
            var prmapping = string.Empty;

            try
            {
                ptmapping = (from map in contexto.ST_GL_MAPPING_POLICY_TYPE
                             where map.POLICY_TYPE.Trim() == policy_type.Trim()
                             select map.GL_MAPPING.Trim()).FirstOrDefault();

                prmapping = (from map in contexto.ST_GL_MAPPING_VEHICLE_TYPE
                             where map.VEHICLE_TYPE.Trim() == vehicle_type.Trim()
                             select map.GL_MAPPING.Trim()).FirstOrDefault();

                if (string.IsNullOrEmpty(ptmapping))
                {
                    log.WriteLog(1, "Póliza no tiene Policy Type");
                }

                if (string.IsNullOrEmpty(prmapping))
                {
                    log.WriteLog(1, "Póliza no tiene Vehicle Type");
                }

                return prmapping.Trim() + ptmapping.Trim();

                //return prmapping.Trim() + ptmapping.Trim(); 
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }

        /// <summary>
        /// Obtiene el codigo de zona
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        private string GetZoneMapping(string supervisor)
        {
            var contexto = new ATLANEntities();
            var supervisormapping = string.Empty;
            try
            {
                supervisormapping = (from map in contexto.ST_GL_MAPPING_SUPERVISOR
                                     where map.SUPERVISOR.Trim() == supervisor.Trim()
                                     select map.ZONE_MAPPING.Trim()).FirstOrDefault();

                if (string.IsNullOrEmpty(supervisormapping))
                {
                    log.WriteLog(1, "Poliza no tiene Zona");
                }
                return supervisormapping;
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: ", 3, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 3, ex);
                throw;
            }
            finally
            {
                contexto.Dispose();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supervisor"></param>
        /// <returns></returns>
        private string GetZoneMappingByCode(int supervisor)
        {
            var contexto = new ATLANEntities();
            var supervisormapping = string.Empty;
            try
            {
                supervisormapping = (from map in contexto.ST_GL_MAPPING_SUPERVISOR
                                     where map.SUPERVISOR_CODE == supervisor
                                     select map.ZONE_MAPPING.Trim()).FirstOrDefault();

                if (string.IsNullOrEmpty(supervisormapping))
                {
                    log.WriteLog(1, "Poliza no tiene supervisor Asignado");
                }
                return supervisormapping;
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: ", 3, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 3, ex);
                throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }

        /// <summary>
        /// Obtiene el codigo de Pais
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        private string GetCountryMapping(string country)
        {
            var contexto = new ATLANEntities();
            var countrymapping = string.Empty;

            try
            {
                countrymapping = (from map in contexto.ST_GL_MAPPING_COUNTRY
                                  where map.COUNTRY.Trim() == country.Trim()
                                  select map.GL_MAPPING.Trim()).FirstOrDefault();
                if (string.IsNullOrEmpty(countrymapping))
                {
                    log.WriteLog(1, "Poliza no tiene pais asignado");
                }


                return countrymapping.Trim();
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: ", 3, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 3, ex);
                throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }

        /// <summary>
        /// Obtiene el codigo de Canal
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        private string GetChannelMapping(string channel)
        {
            var contexto = new ATLANEntities();
            var channelmapping = string.Empty;
            try
            {
                channelmapping = (from map in contexto.ST_GL_MAPPING_CHANNEL
                                  where map.CHANNEL.Trim() == channel.Trim()
                                  select map.GL_MAPPING.Trim()).FirstOrDefault();
                if (string.IsNullOrEmpty(channelmapping))
                {
                    log.WriteLog(1, "Poliza no tiene asignado Canal");
                }

                return channelmapping.Trim();
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: ", 3, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 3, ex);
                throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }

        /// <summary>
        /// Obtiene el codigo de Supervisor
        /// </summary>
        /// <param name="supervisor"></param>
        /// <returns></returns>
        private string GetSupervisorMapping(string supervisor)
        {
            var contexto = new ATLANEntities();
            var supervisormapping = string.Empty;
            try
            {
                supervisormapping = (from map in contexto.ST_GL_MAPPING_SUPERVISOR
                                     where map.SUPERVISOR.Trim() == supervisor.Trim()
                                     select map.GL_MAPPING.Trim()).FirstOrDefault();

                if (string.IsNullOrEmpty(supervisormapping))
                {
                    log.WriteLog(1, "Poliza no tiene supervisor");
                }
                return supervisormapping;
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: ", 3, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 3, ex);
                throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supervisor"></param>
        /// <returns></returns>
        private string GetSupervisorMappingByCode(int supervisor)
        {
            var contexto = new ATLANEntities();
            var supervisormapping = string.Empty;
            try
            {
                supervisormapping = (from map in contexto.ST_GL_MAPPING_SUPERVISOR
                                     where map.SUPERVISOR_CODE == supervisor
                                     select map.GL_MAPPING.Trim()).FirstOrDefault();

                if (string.IsNullOrEmpty(supervisormapping))
                {
                    log.WriteLog(1, "Poliza no tiene supervisor Asignado");
                }
                return supervisormapping;
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: ", 3, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 3, ex);
                throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }

        /// <summary>
        /// Obtiene el segmento 1
        /// </summary>
        /// <param name="segment3"></param>
        /// <param name="segment4"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetSegment1(string segment3, string segment4, int type)
        {
            var contexto = new ATLANEntities();

            var segment1 = string.Empty;
            var segment2 = string.Empty;
            ST_GL_MAPPING_ACCOUNT_SETTINGS config_mapping;

            try
            {
                config_mapping = (from map in contexto.ST_GL_MAPPING_ACCOUNT_SETTINGS
                                  where map.LINE_OF_BUSINESS.Trim() == segment3.Trim() && map.PRODUCT.Trim() == segment4.Trim() && map.TYPEID == type
                                  select map).FirstOrDefault();


                if (config_mapping == null)
                {
                    log.WriteLog(1, "No existe una configuración para la linea de negocio " + segment3.ToString() + " y producto: " + segment4.ToString());
                }
                return config_mapping.SEGMENT1.Trim();
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: ", 3, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 3, ex);
                throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="IsLocal"></param>
        /// <param name="IsFacult"></param>
        /// <returns></returns>
        private string GetSegment1Reinsurance(string segment3, string segment4, bool IsLocal, bool IsFacult)
        {
            var contexto = new ATLANEntities();

            var type = 0;

            if (IsLocal && !IsFacult)
            {
                type = 10;
            }
            if (IsLocal && IsFacult)
            {
                type = 14;
            }
            if (!IsLocal && !IsFacult)
            {
                type = 13;
            }

            if (!IsLocal && IsFacult)
            {
                type = 15;
            }

            try
            {
                //var Cuenta = (from cuentas in contexto.ST_GL_MAPPING_REINSURANCE
                //              where cuentas.IsLocal == IsLocal && cuentas.IsFacult == IsFacult
                //              select cuentas.SEGMENT1).FirstOrDefault();


                var Config_Mappging = (from map in contexto.ST_GL_MAPPING_ACCOUNT_SETTINGS
                                       where map.LINE_OF_BUSINESS.Trim() == segment3.Trim() && map.PRODUCT.Trim() == segment4.Trim() && map.TYPEID == type
                                       select map.SEGMENT1).FirstOrDefault();
                return Config_Mappging;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }

        /// <summary>
        /// Obtiene el segmento 2
        /// </summary>
        /// <param name="segment3"></param>
        /// <param name="segment4"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetSegment2(string segment3, string segment4, int type)
        {
            var contexto = new ATLANEntities();

            var segment1 = string.Empty;
            var segment2 = string.Empty;
            ST_GL_MAPPING_ACCOUNT_SETTINGS config_mapping;

            try
            {
                config_mapping = (from map in contexto.ST_GL_MAPPING_ACCOUNT_SETTINGS
                                  where map.LINE_OF_BUSINESS.Trim() == segment3.Trim() && map.PRODUCT.Trim() == segment4.Trim() && map.TYPEID == type
                                  select map).FirstOrDefault();

                if (config_mapping == null)
                {
                    log.WriteLog(1, "No existe una configuración para la linea de negocio " + segment3.ToString() + " y producto: " + segment4.ToString());
                }

                return config_mapping.SEGMENT2.Trim();
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: ", 3, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 3, ex);
                throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }


        /// <summary>
        /// Obtiene el codigo de la Cobertura
        /// </summary>
        /// <param name="coverage"></param>
        /// <returns></returns>
        private string GetCoverageMapping(string coverage)
        {
            var contexto = new ATLANEntities();
            var coveragemapping = string.Empty;
            try
            {
                coveragemapping = (from map in contexto.ST_GL_MAPPING_COVERAGE
                                   where map.COVERAGE == coverage
                                   select map.GL_MAPPING.Trim()).FirstOrDefault();
                if (string.IsNullOrEmpty(coveragemapping))
                {
                    AddCoverage(coverage);
                    log.WriteLog(1, "No se encontró la covertura asignada " + coverage);
                }
                return coveragemapping;
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: ", 3, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 3, ex);
                throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }


        /// <summary>
        /// Agrega la descripcion de la cobertura si esta no es encontrada
        /// </summary>
        /// <param name="coverage"></param>
        private void AddCoverage(string coverage)
        {
            var contexto = new ATLANEntities();
            var glcoverage = new ST_GL_MAPPING_COVERAGE();

            try
            {
                glcoverage.COVERAGE = coverage;

                contexto.ST_GL_MAPPING_COVERAGE.Add(glcoverage);

                contexto.SaveChanges();
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: ", 3, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 3, ex);
                throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Account"></param>
        private void CreateAccount(string Account)
        {
            var contexto = new ATLANEntities();
            var TpcBalance = 0;
            var PstType = 0;

            if (!ExistsAccount(Account))
            {
                try
                {
                    //if (Account.Substring(1, 1) == "1")
                    //{
                    //    PstType = 0;
                    //    TpcBalance = 0;
                    //}

                    //if (Account.Substring(1, 1) == "2")
                    //{
                    //    PstType = 0;
                    //    TpcBalance = 1;
                    //}

                    //if (Account.Substring(1, 1) == "4")
                    //{
                    //    PstType = 1;
                    //    TpcBalance = 1;
                    //}

                    //if (Account.Substring(1, 1) == "5")
                    //{
                    //    PstType = 1;
                    //    TpcBalance = 0;
                    //}
                    if (Account.Substring(1, 1) == "1")
                    {
                        PstType = 0;
                        TpcBalance = 0;
                    }

                    if (Account.Substring(1, 1) == "2")
                    {
                        PstType = 0;
                        TpcBalance = 1;
                    }

                    if (Account.Substring(1, 1) == "4")
                    {
                        PstType = 1;
                        TpcBalance = 1;
                    }

                    if (Account.Substring(1, 1) == "5")
                    {
                        PstType = 1;
                        TpcBalance = 0;
                    }
                    contexto.pSP_CREAR_CUENTAS(Account, "-----------------", TpcBalance, PstType, 1, "Nonfinancial Accounts                              ", 0);


                }
                catch (EntityException ex)
                {
                    log.LogExeption("Ocurrió un error al crear la cuenta contable: ", 3, ex);
                    //throw;
                }
                catch (Exception ex)
                {
                    log.LogExeption("Ocurrió un error al crear la cuenta contable: ", 3, ex);
                    // throw;
                }
                finally
                {
                    contexto.Dispose();
                }
            }


            //contexto.pSP_CREAR_CUENTAS()
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        private bool ExistsAccount(string Account)
        {
            var contexto = new ATLANEntities();

            try
            {
                //var conteo = (from cuentas in contexto.Accounts
                //              where cuentas.Account_Number.Contains(Account)
                //              select cuentas).Count();

                //if (conteo > 0)
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
                return contexto.usp_ST_GL_VERIFY_ACCOUNT_EXISTS(Account.Trim()).FirstOrDefault().GetValueOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeAccount"></param>
        /// <param name="segment1"></param>
        /// <param name="segment2"></param>
        /// <param name="segment3"></param>
        /// <param name="segment4"></param>
        /// <returns></returns>
        private string GetUnitAccount(int typeAccount, string segment1, string segment2, string segment3, string segment4)
        {
            var contexto = new ATLANEntities();
            var UnitAccount = string.Empty;

            try
            {
                UnitAccount = (from accounts in contexto.ST_GL_MAPPING_ACCOUNT_SETTINGS
                               where accounts.SEGMENT1.Trim() == segment1.Trim() &&
                                     accounts.SEGMENT2.Trim() == segment2.Trim() &&
                                     accounts.LINE_OF_BUSINESS.Trim() == segment3.Trim() &&
                                     accounts.PRODUCT.Trim() == segment4.Trim()
                               select accounts.UNIT_ACCOUNT1).FirstOrDefault().Trim();

                return UnitAccount;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                contexto.Dispose();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeAccount"></param>
        /// <returns></returns>
        public string GetUnitAccount(int typeAccount)
        {
            var contexto = new ATLANEntities();
            var UnitAccount = string.Empty;

            try
            {
                UnitAccount = (from accounts in contexto.ST_GL_MAPPING_ACCOUNT_SETTINGS
                               where accounts.TYPEID == typeAccount
                               select accounts.UNIT_ACCOUNT1).FirstOrDefault().Trim();

                return UnitAccount;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }

    }
}

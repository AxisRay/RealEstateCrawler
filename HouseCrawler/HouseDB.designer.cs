﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace HouseCrawler
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="RealEstate")]
	public partial class HouseDBDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region 可扩展性方法定义
    partial void OnCreated();
    partial void InsertLoupanSummary(LoupanSummary instance);
    partial void UpdateLoupanSummary(LoupanSummary instance);
    partial void DeleteLoupanSummary(LoupanSummary instance);
    partial void InsertHouseDetail(HouseDetail instance);
    partial void UpdateHouseDetail(HouseDetail instance);
    partial void DeleteHouseDetail(HouseDetail instance);
    #endregion
		
		public HouseDBDataContext() : 
				base(global::HouseCrawler.Properties.Settings.Default.RealEstateConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public HouseDBDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public HouseDBDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public HouseDBDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public HouseDBDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<LoupanSummary> LoupanSummary
		{
			get
			{
				return this.GetTable<LoupanSummary>();
			}
		}
		
		public System.Data.Linq.Table<HouseDetail> HouseDetail
		{
			get
			{
				return this.GetTable<HouseDetail>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.LoupanSummary")]
	public partial class LoupanSummary : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _ID;
		
		private string _LoupanName;
		
		private System.Nullable<int> _Price;
		
		private string _Address;
		
		private string _Region;
		
		private string _City;
		
		private string _Url;
		
    #region 可扩展性方法定义
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(string value);
    partial void OnIDChanged();
    partial void OnLoupanNameChanging(string value);
    partial void OnLoupanNameChanged();
    partial void OnPriceChanging(System.Nullable<int> value);
    partial void OnPriceChanged();
    partial void OnAddressChanging(string value);
    partial void OnAddressChanged();
    partial void OnRegionChanging(string value);
    partial void OnRegionChanged();
    partial void OnCityChanging(string value);
    partial void OnCityChanged();
    partial void OnUrlChanging(string value);
    partial void OnUrlChanged();
    #endregion
		
		public LoupanSummary()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", DbType="NChar(10) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LoupanName", DbType="NVarChar(255)")]
		public string LoupanName
		{
			get
			{
				return this._LoupanName;
			}
			set
			{
				if ((this._LoupanName != value))
				{
					this.OnLoupanNameChanging(value);
					this.SendPropertyChanging();
					this._LoupanName = value;
					this.SendPropertyChanged("LoupanName");
					this.OnLoupanNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Price", DbType="Int")]
		public System.Nullable<int> Price
		{
			get
			{
				return this._Price;
			}
			set
			{
				if ((this._Price != value))
				{
					this.OnPriceChanging(value);
					this.SendPropertyChanging();
					this._Price = value;
					this.SendPropertyChanged("Price");
					this.OnPriceChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Address", DbType="NVarChar(255)")]
		public string Address
		{
			get
			{
				return this._Address;
			}
			set
			{
				if ((this._Address != value))
				{
					this.OnAddressChanging(value);
					this.SendPropertyChanging();
					this._Address = value;
					this.SendPropertyChanged("Address");
					this.OnAddressChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Region", DbType="NVarChar(255)")]
		public string Region
		{
			get
			{
				return this._Region;
			}
			set
			{
				if ((this._Region != value))
				{
					this.OnRegionChanging(value);
					this.SendPropertyChanging();
					this._Region = value;
					this.SendPropertyChanged("Region");
					this.OnRegionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_City", DbType="NVarChar(255)")]
		public string City
		{
			get
			{
				return this._City;
			}
			set
			{
				if ((this._City != value))
				{
					this.OnCityChanging(value);
					this.SendPropertyChanging();
					this._City = value;
					this.SendPropertyChanged("City");
					this.OnCityChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Url", DbType="NVarChar(255)")]
		public string Url
		{
			get
			{
				return this._Url;
			}
			set
			{
				if ((this._Url != value))
				{
					this.OnUrlChanging(value);
					this.SendPropertyChanging();
					this._Url = value;
					this.SendPropertyChanged("Url");
					this.OnUrlChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.HouseDetail")]
	public partial class HouseDetail : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ID;
		
		private string _houseid;
		
		private string _dongid;
		
		private string _newcode;
		
		private string _projname;
		
		private string _dongname;
		
		private string _address;
		
		private System.Nullable<int> _room;
		
		private System.Nullable<int> _ting;
		
		private System.Nullable<int> _wei;
		
		private System.Nullable<int> _chu;
		
		private System.Nullable<float> _jianzhumianji;
		
		private string _houselocation;
		
		private string _district;
		
		private System.Nullable<float> _tehui_price;
		
		private System.Nullable<float> _price_s;
		
		private string _price_s_type;
		
		private System.Nullable<float> _price_t;
		
		private string _price_t_type;
		
    #region 可扩展性方法定义
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(int value);
    partial void OnIDChanged();
    partial void OnhouseidChanging(string value);
    partial void OnhouseidChanged();
    partial void OndongidChanging(string value);
    partial void OndongidChanged();
    partial void OnnewcodeChanging(string value);
    partial void OnnewcodeChanged();
    partial void OnprojnameChanging(string value);
    partial void OnprojnameChanged();
    partial void OndongnameChanging(string value);
    partial void OndongnameChanged();
    partial void OnaddressChanging(string value);
    partial void OnaddressChanged();
    partial void OnroomChanging(System.Nullable<int> value);
    partial void OnroomChanged();
    partial void OntingChanging(System.Nullable<int> value);
    partial void OntingChanged();
    partial void OnweiChanging(System.Nullable<int> value);
    partial void OnweiChanged();
    partial void OnchuChanging(System.Nullable<int> value);
    partial void OnchuChanged();
    partial void OnjianzhumianjiChanging(System.Nullable<float> value);
    partial void OnjianzhumianjiChanged();
    partial void OnhouselocationChanging(string value);
    partial void OnhouselocationChanged();
    partial void OndistrictChanging(string value);
    partial void OndistrictChanged();
    partial void Ontehui_priceChanging(System.Nullable<float> value);
    partial void Ontehui_priceChanged();
    partial void Onprice_sChanging(System.Nullable<float> value);
    partial void Onprice_sChanged();
    partial void Onprice_s_typeChanging(string value);
    partial void Onprice_s_typeChanged();
    partial void Onprice_tChanging(System.Nullable<float> value);
    partial void Onprice_tChanged();
    partial void Onprice_t_typeChanging(string value);
    partial void Onprice_t_typeChanged();
    #endregion
		
		public HouseDetail()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_houseid", DbType="NVarChar(255)")]
		public string houseid
		{
			get
			{
				return this._houseid;
			}
			set
			{
				if ((this._houseid != value))
				{
					this.OnhouseidChanging(value);
					this.SendPropertyChanging();
					this._houseid = value;
					this.SendPropertyChanged("houseid");
					this.OnhouseidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_dongid", DbType="NVarChar(255)")]
		public string dongid
		{
			get
			{
				return this._dongid;
			}
			set
			{
				if ((this._dongid != value))
				{
					this.OndongidChanging(value);
					this.SendPropertyChanging();
					this._dongid = value;
					this.SendPropertyChanged("dongid");
					this.OndongidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_newcode", DbType="NChar(10)")]
		public string newcode
		{
			get
			{
				return this._newcode;
			}
			set
			{
				if ((this._newcode != value))
				{
					this.OnnewcodeChanging(value);
					this.SendPropertyChanging();
					this._newcode = value;
					this.SendPropertyChanged("newcode");
					this.OnnewcodeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_projname", DbType="NVarChar(255)")]
		public string projname
		{
			get
			{
				return this._projname;
			}
			set
			{
				if ((this._projname != value))
				{
					this.OnprojnameChanging(value);
					this.SendPropertyChanging();
					this._projname = value;
					this.SendPropertyChanged("projname");
					this.OnprojnameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_dongname", DbType="NVarChar(255)")]
		public string dongname
		{
			get
			{
				return this._dongname;
			}
			set
			{
				if ((this._dongname != value))
				{
					this.OndongnameChanging(value);
					this.SendPropertyChanging();
					this._dongname = value;
					this.SendPropertyChanged("dongname");
					this.OndongnameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_address", DbType="NVarChar(255)")]
		public string address
		{
			get
			{
				return this._address;
			}
			set
			{
				if ((this._address != value))
				{
					this.OnaddressChanging(value);
					this.SendPropertyChanging();
					this._address = value;
					this.SendPropertyChanged("address");
					this.OnaddressChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_room", DbType="Int")]
		public System.Nullable<int> room
		{
			get
			{
				return this._room;
			}
			set
			{
				if ((this._room != value))
				{
					this.OnroomChanging(value);
					this.SendPropertyChanging();
					this._room = value;
					this.SendPropertyChanged("room");
					this.OnroomChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ting", DbType="Int")]
		public System.Nullable<int> ting
		{
			get
			{
				return this._ting;
			}
			set
			{
				if ((this._ting != value))
				{
					this.OntingChanging(value);
					this.SendPropertyChanging();
					this._ting = value;
					this.SendPropertyChanged("ting");
					this.OntingChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_wei", DbType="Int")]
		public System.Nullable<int> wei
		{
			get
			{
				return this._wei;
			}
			set
			{
				if ((this._wei != value))
				{
					this.OnweiChanging(value);
					this.SendPropertyChanging();
					this._wei = value;
					this.SendPropertyChanged("wei");
					this.OnweiChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_chu", DbType="Int")]
		public System.Nullable<int> chu
		{
			get
			{
				return this._chu;
			}
			set
			{
				if ((this._chu != value))
				{
					this.OnchuChanging(value);
					this.SendPropertyChanging();
					this._chu = value;
					this.SendPropertyChanged("chu");
					this.OnchuChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_jianzhumianji", DbType="Real")]
		public System.Nullable<float> jianzhumianji
		{
			get
			{
				return this._jianzhumianji;
			}
			set
			{
				if ((this._jianzhumianji != value))
				{
					this.OnjianzhumianjiChanging(value);
					this.SendPropertyChanging();
					this._jianzhumianji = value;
					this.SendPropertyChanged("jianzhumianji");
					this.OnjianzhumianjiChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_houselocation", DbType="NVarChar(255)")]
		public string houselocation
		{
			get
			{
				return this._houselocation;
			}
			set
			{
				if ((this._houselocation != value))
				{
					this.OnhouselocationChanging(value);
					this.SendPropertyChanging();
					this._houselocation = value;
					this.SendPropertyChanged("houselocation");
					this.OnhouselocationChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_district", DbType="NVarChar(255)")]
		public string district
		{
			get
			{
				return this._district;
			}
			set
			{
				if ((this._district != value))
				{
					this.OndistrictChanging(value);
					this.SendPropertyChanging();
					this._district = value;
					this.SendPropertyChanged("district");
					this.OndistrictChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_tehui_price", DbType="Real")]
		public System.Nullable<float> tehui_price
		{
			get
			{
				return this._tehui_price;
			}
			set
			{
				if ((this._tehui_price != value))
				{
					this.Ontehui_priceChanging(value);
					this.SendPropertyChanging();
					this._tehui_price = value;
					this.SendPropertyChanged("tehui_price");
					this.Ontehui_priceChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_price_s", DbType="Real")]
		public System.Nullable<float> price_s
		{
			get
			{
				return this._price_s;
			}
			set
			{
				if ((this._price_s != value))
				{
					this.Onprice_sChanging(value);
					this.SendPropertyChanging();
					this._price_s = value;
					this.SendPropertyChanged("price_s");
					this.Onprice_sChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_price_s_type", DbType="NVarChar(255)")]
		public string price_s_type
		{
			get
			{
				return this._price_s_type;
			}
			set
			{
				if ((this._price_s_type != value))
				{
					this.Onprice_s_typeChanging(value);
					this.SendPropertyChanging();
					this._price_s_type = value;
					this.SendPropertyChanged("price_s_type");
					this.Onprice_s_typeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_price_t", DbType="Real")]
		public System.Nullable<float> price_t
		{
			get
			{
				return this._price_t;
			}
			set
			{
				if ((this._price_t != value))
				{
					this.Onprice_tChanging(value);
					this.SendPropertyChanging();
					this._price_t = value;
					this.SendPropertyChanged("price_t");
					this.Onprice_tChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_price_t_type", DbType="NVarChar(255)")]
		public string price_t_type
		{
			get
			{
				return this._price_t_type;
			}
			set
			{
				if ((this._price_t_type != value))
				{
					this.Onprice_t_typeChanging(value);
					this.SendPropertyChanging();
					this._price_t_type = value;
					this.SendPropertyChanged("price_t_type");
					this.Onprice_t_typeChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591

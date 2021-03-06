﻿/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 19:23
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using StangradCRM.Core;
using StangradCRM.Model;

namespace StangradCRM.ViewModel
{
	/// <summary>
	/// Description of ComplectationViewModel.
	/// </summary>
	public class ComplectationViewModel : Core.BaseViewModel, Core.IViewModel
	{
		private static ComplectationViewModel _instance = null;
		private TSObservableCollection<Complectation> _collection
			= new TSObservableCollection<Complectation>();
		public TSObservableCollection<Complectation> Collection
		{
			get
			{
				return _collection;
			}
			set
			{
				_collection = value;
				RaisePropertyChanged("Collection", value);
			}
		}
		
		private ComplectationViewModel() { load(); }
		
		public static ComplectationViewModel instance ()
		{
			if(_instance == null)
			{
				_instance = new ComplectationViewModel();
			}
			return _instance;
		}		
		
		
		public bool @add<T>(T modelItem)
		{
			Complectation complectation = modelItem as Complectation;
			if(complectation == null)
			{
				complectation.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			Complectation exist = getById(complectation.Id);
			if(exist != null || _collection.Contains(complectation))
			{
				complectation.LastError = "Днная запись уже есть в коллекции.";
				return false;
			}
			_collection.Add(complectation);
			complectation.loadedItemInitProperty();
			return true;
		}
		
		public bool @remove<T>(T modelItem)
		{
			Complectation complectation = modelItem as Complectation;
			if(complectation == null)
			{
				complectation.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			if(!_collection.Contains(complectation)) return true;
			return _collection.Remove(complectation);
		}
		
		public Core.Model getItem(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public TSObservableCollection<Complectation> getByEquipmentBidId (int equipmentBidId)
		{
			List<Complectation> complectation = _collection.Where(x => x.Id_equipment_bid == equipmentBidId).ToList();
			return new TSObservableCollection<Complectation>(complectation);
		}
		
		public Complectation getById(int complectationId)
		{
			return _collection.Where(x => x.Id == complectationId).FirstOrDefault();
		}
				
		protected override void removeAllItems()
		{
			_collection.ToList().ForEach(x => remove(x));
		}
		
		protected override void load() {}
	}
}

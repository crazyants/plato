using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace Plato.Layout.Adaptors
{

    public interface IViewAdaptorBuilder
    {

        object Model { get; }

        void UpdateModel(object model);

        Task<IViewAdaptorBuilder> OnDisplay(Action<IViewAdaptorBuilder> context);
        
        void AlterContent(Func<IHtmlContent, IHtmlContent> action);

        IEnumerable<Func<IHtmlContent, IHtmlContent>> CotentAlterations { get; }

    }


    public class ViewAdaptorBuilder : IViewAdaptorBuilder
    {

        private object _model;

        public object Model => _model;

        public void UpdateModel(object model)
        {
            _model = model;
        }

        public Task<IViewAdaptorBuilder> OnDisplay(Action<IViewAdaptorBuilder> context)
        {
            throw new NotImplementedException();
        }

        private List<Func<IHtmlContent, IHtmlContent>> _cotentAlteratins;

        public IEnumerable<Func<IHtmlContent, IHtmlContent>> CotentAlterations => _cotentAlteratins;

        public void AlterContent(Func<IHtmlContent, IHtmlContent> alteration)
        {
            if (_cotentAlteratins == null)
            {
                _cotentAlteratins = new List<Func<IHtmlContent, IHtmlContent>>();
                _cotentAlteratins.Add(alteration);
            }
          
        }
    }
}

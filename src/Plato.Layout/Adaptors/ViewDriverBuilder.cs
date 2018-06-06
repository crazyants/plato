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

        void AdaptOutput(Func<IHtmlContent, IHtmlContent> action);

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

        private List<Func<IHtmlContent, IHtmlContent>> _cotentAlteratins;

        public IEnumerable<Func<IHtmlContent, IHtmlContent>> CotentAlterations => _cotentAlteratins;

        public void AdaptOutput(Func<IHtmlContent, IHtmlContent> alteration)
        {
            if (_cotentAlteratins == null)
            {
                _cotentAlteratins = new List<Func<IHtmlContent, IHtmlContent>>();
                _cotentAlteratins.Add(alteration);
            }
          
        }
    }
}

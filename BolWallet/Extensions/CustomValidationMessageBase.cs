using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BolWallet
{
    public class CustomValidationMessageBase<TValue> : ComponentBase, IDisposable
    {
        private EditContext? _previousEditContext;
        private Expression<Func<TValue>>? _previousFieldAccessor;
        private readonly EventHandler<ValidationStateChangedEventArgs>? _validationStateChangedHandler;
        private FieldIdentifier _fieldIdentifier;

        /// <summary>
        /// Gets or sets a collection of additional attributes that will be applied to the created <c>div</c> element.
        /// </summary>
        [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

        [CascadingParameter] EditContext CurrentEditContext { get; set; } = default!;

        /// <summary>
        /// Specifies the field for which validation messages should be displayed.
        /// </summary>
        [Parameter] public Expression<Func<TValue>>? For { get; set; }
        [Parameter] public string CustomField { get; set; }

        public IEnumerable<string> ValidationMessages;

        /// <summary>`
        /// Constructs an instance of <see cref="ValidationMessage{TValue}"/>.
        /// </summary>
        public CustomValidationMessageBase()
        {
            _validationStateChangedHandler = (sender, eventArgs) => StateHasChanged();
        }

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException($"{GetType()} requires a cascading parameter " +
                    $"of type {nameof(EditContext)}. For example, you can use {GetType()} inside " +
                    $"an {nameof(EditForm)}.");
            }

            if (!string.IsNullOrEmpty(CustomField))
            {
                //ValidationMessages = CurrentEditContext.GetData(CustomField);
            }
            else if (For == null) // Not possible except if you manually specify T
            {

                throw new InvalidOperationException($"{GetType()} requires a value for the " +
                    $"{nameof(For)} parameter.");
            }
            else if (For != _previousFieldAccessor)
            {
                _fieldIdentifier = FieldIdentifier.Create(For);
                _previousFieldAccessor = For;
                if(_fieldIdentifier.Model is BaseProperty bm)
                {
                    ValidationMessages = new List<string> () { bm.ErrorMessage };
                    
                }
                //ValidationMessages = CurrentEditContext.GetValidationMessages(_fieldIdentifier);
                //ValidationMessages = CurrentEditContext.GetData(_fieldIdentifier.FieldName);
            }

            if (CurrentEditContext != _previousEditContext)
            {
                DetachValidationStateChangedListener();
                CurrentEditContext.OnValidationStateChanged += _validationStateChangedHandler;
                _previousEditContext = CurrentEditContext;
            }


        }

        /// <inheritdoc />
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            foreach (var message in ValidationMessages)
            {
                builder.OpenElement(0, "div");
                builder.AddMultipleAttributes(1, AdditionalAttributes);
                builder.AddAttribute(2, "class", "validation-message");
                builder.AddContent(3, message);
                builder.CloseElement();
            }
        }

        /// <summary>
        /// Called to dispose this instance.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if called within <see cref="IDisposable.Dispose"/>.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        void IDisposable.Dispose()
        {
            DetachValidationStateChangedListener();
            Dispose(disposing: true);
        }

        private void DetachValidationStateChangedListener()
        {
            if (_previousEditContext != null)
            {
                _previousEditContext.OnValidationStateChanged -= _validationStateChangedHandler;
            }
        }
    }
}


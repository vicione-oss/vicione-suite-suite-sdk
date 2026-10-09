import eslintConfigXo from 'eslint-config-xo';

const indent = 4;

const config = [
    {
        ignores: [
            '**/wwwroot/_framework/*.js',
            '**/wwwroot/_content/*.js',
            '**/wwwroot/js/*.js',
            '**/*.razor.js',
            '**/bin',
            '**/obj',
            '**/packages',
            'tests/**/*.js',
            '**/ReconnectModal.razor.js',
            '**/*.css',
            '**/*.html',
            '**/*.json',
            '**/*.md'
        ]
    },
    ...eslintConfigXo({ space: indent }),
    {
        languageOptions: {
            parserOptions: {
                projectService: {
                    allowDefaultProject: ['eslint.config.mjs']
                }
            }
        }
    },

    // Formatting shared by all JavaScript / TypeScript code the solution lints
    {
        files: ['**/*.ts', '**/eslint.config.mjs'],
        rules: {
            '@stylistic/comma-dangle': ['error', 'never'],
            '@stylistic/object-curly-spacing': ['error', 'always']
        }
    },

    {
        files: ['**/*.ts'],
        rules: {
            'no-unused-vars': 'error',
            curly: ['error', 'multi-or-nest', 'consistent'],
            '@stylistic/padded-blocks': 'off',
            '@stylistic/indent': ['error', indent],
            '@stylistic/indent-binary-ops': ['error', indent],
            '@stylistic/function-paren-newline': ['error', 'consistent'],
            '@stylistic/curly-newline': ['error', { minElements: 1 }],
            '@stylistic/operator-linebreak': ['error', 'after'],
            '@typescript-eslint/no-empty-object-type': ['error', { allowInterfaces: 'with-single-extends' }],
            'max-params': ['warn', 5],

            'import-x/no-absolute-path': 'off',
            'import-x/no-unassigned-import': ['error', { allow: ['**/event-target-mixins.js'] }],

            // Each guard carries its own reason, and merging them leaves a trailing
            // comment describing the whole condition or rendering the whole
            // condition unreadable, hence this rule is off
            'unicorn/prefer-combined-guards': 'off',

            // Enforcing early returns more than often results in unreadable code
            // because code flow cannot be analyzed at the first glance, hence this
            // rule is off
            'unicorn/prefer-early-return': 'off',

            'unicorn/filename-case': 'off'
        }
    },

    // Mixin classes use the `this` parameter pattern, so members referenced via
    // `this` are declared on the target type, not on the mixin class itself.
    {
        files: ['**/*Mixins.ts'],
        rules: {
            'unicorn/no-undeclared-class-members': 'off'
        }
    },

    // Linting rule adjustments for the configuration files themselves
    {
        files: ['**/eslint.config.mjs'],
        rules: {
            '@typescript-eslint/no-unsafe-assignment': 'off',
            '@typescript-eslint/no-unsafe-call': 'off',
            '@typescript-eslint/no-unsafe-member-access': 'off',
            '@typescript-eslint/naming-convention': 'off'
        }
    }
];

export default config;
